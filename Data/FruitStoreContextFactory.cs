using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyStore.Data
{
    public class FruitStoreContextFactory : IDesignTimeDbContextFactory<FruitStoreContext>
    {
        public FruitStoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FruitStoreContext>();
            // Use configuration/environment variable instead of hardcoded connection string on cloud
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ?? "";
            optionsBuilder.UseSqlServer(connectionString);
            return new FruitStoreContext(optionsBuilder.Options);
        }
    }

}
