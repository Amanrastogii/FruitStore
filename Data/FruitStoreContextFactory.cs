using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MyStore.Data
{
    public class FruitStoreContextFactory : IDesignTimeDbContextFactory<FruitStoreContext>
    {
        public FruitStoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FruitStoreContext>();

            // First, try to get from environment variable (for cloud/docker/CI/CD)
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            // Fallback to local configuration if not set (for local development)
            if (string.IsNullOrEmpty(connectionString))
            {
                // Try to load from appsettings.Development.json
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .Build();

                connectionString = config.GetConnectionString("DefaultConnection");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "No connection string configured. " +
                    "Set ConnectionStrings__DefaultConnection as an environment variable or in appsettings.Development.json for local development.");
            }

            // Use SQL Server for local and production (not Npgsql)
            optionsBuilder.UseSqlServer(connectionString);

            return new FruitStoreContext(optionsBuilder.Options);
        }
    }
}
