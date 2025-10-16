using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
            // Convert postgres://user:password@host:port/database to Npgsql format
            if (postgresUrl.StartsWith("postgres://"))
            {
                var uri = new Uri(postgresUrl);
                return $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={uri.UserInfo.Split(':')};Password={uri.UserInfo.Split(':')};SSL Mode=Require;Trust Server Certificate=true";
            }
            return postgresUrl; // Already in correct format
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
