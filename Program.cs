using MyStore.Data;
using MyStore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace MyStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("ENTRY POINT: Main started");
            try
            {
                Console.WriteLine("Creating builder...");
                var builder = WebApplication.CreateBuilder(args);

                Console.WriteLine("Configuring services...");
                builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

                // CRITICAL: Use PostgreSQL instead of SQL Server
                var connectionString = GetConnectionString(builder.Configuration);
                Console.WriteLine($"Using connection string: {MaskConnectionString(connectionString)}");

                builder.Services.AddDbContext<FruitStoreContext>(options =>
                    options.UseNpgsql(connectionString, // Changed from UseSqlServer to UseNpgsql
                        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null)));

                builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                });
                builder.Services.AddHttpContextAccessor();

                // ====== Swagger Setup ======
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "MyStore API",
                        Version = "v1",
                        Description = "API for Fruit Store CRUD operations"
                    });
                });
                // ===========================

                // CRITICAL: Configure Kestrel for Render deployment
                var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
                Console.WriteLine($"Configuring Kestrel to listen on port: {port}");
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(int.Parse(port));
                });

                Console.WriteLine("Building app...");
                var app = builder.Build();

                Console.WriteLine("Configuring middleware...");
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // Don't use HSTS in production on Render (they handle HTTPS)
                    // app.UseHsts();
                }

                // Don't redirect to HTTPS - Render handles this
                // app.UseHttpsRedirection();

                app.UseStaticFiles();
                app.UseRouting();
                app.UseSession();
                app.UseAuthorization();

                // ====== Enable Swagger in Production for Testing ======
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyStore API v1");
                    c.RoutePrefix = "swagger";
                });
                // =====================================================

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Admin}/{action=Login}/{id?}");

                // ===== Map API Controllers for Swagger =====
                app.MapControllers(); // Needed for attribute-based routing (like [ApiController])
                // ===========================================

                // ====== AUTO APPLY EF CORE MIGRATIONS ON BOOT (IMPORTANT FOR CLOUD) ======
                using (var scope = app.Services.CreateScope())
                {
                    try
                    {
                        Console.WriteLine("Applying database migrations...");
                        var db = scope.ServiceProvider.GetRequiredService<FruitStoreContext>();
                        db.Database.Migrate();
                        Console.WriteLine("Database migrations completed successfully");
                    }
                    catch (Exception dbEx)
                    {
                        Console.WriteLine($"Database migration failed: {dbEx}");
                        throw;
                    }
                }
                // ========================================================================

                Console.WriteLine("About to call app.Run()");
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal startup error: " + ex);
                throw;
            }
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            // First try to get from environment variable (Render uses this approach)
            var envConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrEmpty(envConnectionString))
            {
                Console.WriteLine("Using DATABASE_URL environment variable");
                return ConvertPostgresUrl(envConnectionString);
            }

            // Fallback to configuration
            var configConnectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(configConnectionString))
            {
                Console.WriteLine("Using DefaultConnection from configuration");
                return configConnectionString;
            }

            throw new InvalidOperationException("No connection string found. Set DATABASE_URL environment variable or DefaultConnection in appsettings.json");
        }

        private static string ConvertPostgresUrl(string postgresUrl)
        {
            // Convert postgres://user:password@host:port/database to Npgsql format
            if (postgresUrl.StartsWith("postgres://") || postgresUrl.StartsWith("postgresql://"))
            {
                var uri = new Uri(postgresUrl);
                var userInfo = uri.UserInfo.Split(':'); // FIXED: Store the split result
                return $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
            }
            return postgresUrl; // Already in correct format
        }

        private static string MaskConnectionString(string connectionString)
        {
            // Mask password for logging
            if (connectionString.Contains("Password="))
            {
                var parts = connectionString.Split(';');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("Password="))
                    {
                        parts[i] = "Password=****";
                    }
                }
                return string.Join(';', parts);
            }
            return connectionString;
        }
    }
}
