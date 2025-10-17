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

            // Use PostgreSQL instead of SQL Server
            var connectionString = GetConnectionString();
            Console.WriteLine($"Design-time context using: {MaskConnectionString(connectionString)}");

            optionsBuilder.UseNpgsql(connectionString); // Changed from UseSqlServer to UseNpgsql

            return new FruitStoreContext(optionsBuilder.Options);
        }

        private string GetConnectionString()
        {
            // First try environment variable (used by Render)
            var envConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrEmpty(envConnectionString))
            {
                return ConvertPostgresUrl(envConnectionString);
            }

            // Try Render's specific environment variable format
            var renderConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            if (!string.IsNullOrEmpty(renderConnectionString))
            {
                return renderConnectionString;
            }

            // Fallback for local development - you may need to set this
            return "Host=localhost;Port=5432;Database=fruitstore_dev;Username=postgres;Password=your_password;";
        }

        private string ConvertPostgresUrl(string postgresUrl)
        {
            if (postgresUrl.StartsWith("postgres://") || postgresUrl.StartsWith("postgresql://"))
            {
                var uri = new Uri(postgresUrl);
                var userInfo = uri.UserInfo.Split(':');

                // CRITICAL FIX: Use default port 5432 if URI doesn't specify one
                var port = uri.Port > 0 ? uri.Port : 5432;

                return $"Host={uri.Host};Port={port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
            }
            return postgresUrl;
        }

        private string MaskConnectionString(string connectionString)
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
