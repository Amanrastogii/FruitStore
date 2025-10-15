using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyStore.Data
{
    public class FruitStoreContextFactory : IDesignTimeDbContextFactory<FruitStoreContext>
    {
        public FruitStoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FruitStoreContext>();
            optionsBuilder.UseSqlServer("Server=LAPTOP-MF7L2NRO;Database=MyStore;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=true;");
            return new FruitStoreContext(optionsBuilder.Options);
        }
    }
}
