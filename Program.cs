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

                // Use SQL Server for local development and production
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Database connection string is not configured. Please set DefaultConnection in your appsettings.Development.json or environment variable.");
                }

                builder.Services.AddDbContext<FruitStoreContext>(options =>
                    options.UseSqlServer(connectionString,
                        sqlOptions => sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null)));

                builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Secure cookies
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

                Console.WriteLine("Building app...");
                var app = builder.Build();

                Console.WriteLine("Configuring middleware...");
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();

                    // Swagger only in development
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyStore API v1");
                        c.RoutePrefix = "swagger";
                    });
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();

                    // Production security headers
                    app.Use(async (context, next) =>
                    {
                        context.Response.Headers.Add("X-Frame-Options", "DENY");
                        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                        await next();
                    });
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();
                app.UseSession();
                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Admin}/{action=Login}/{id?}");

                // ===== Map API Controllers for Swagger =====
                app.MapControllers();

                // Optional: Health check endpoint for deployment validation
                app.MapGet("/health", () => Results.Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    environment = app.Environment.EnvironmentName
                }));

                // ====== AUTO APPLY EF CORE MIGRATIONS ON BOOT (IMPORTANT FOR CLOUD) ======
                try
                {
                    using var scope = app.Services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<FruitStoreContext>();
                    if (db.Database.CanConnect())
                    {
                        db.Database.Migrate();
                        Console.WriteLine("Database migrations applied successfully");
                    }
                    else
                    {
                        Console.WriteLine("Warning: Cannot connect to database - migrations not applied");
                    }
                }
                catch (Exception dbEx)
                {
                    Console.WriteLine("Database migration error: " + dbEx.Message);
                    if (!app.Environment.IsDevelopment())
                        throw;
                }

                Console.WriteLine("About to call app.Run()");
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal startup error: " + ex.Message);
                throw;
            }
        }
    }
}
