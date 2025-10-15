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

                builder.Services.AddDbContext<FruitStoreContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
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
                    // ====== Swagger in Dev Environment ======
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyStore API v1");
                        c.RoutePrefix = "swagger";
                    });
                    // ========================================
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
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
                app.MapControllers(); // Needed for attribute-based routing (like [ApiController])
                // ===========================================

                Console.WriteLine("About to call app.Run()");
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal startup error: " + ex);
                throw;
            }
        }
    }
}
