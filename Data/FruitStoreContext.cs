using Microsoft.EntityFrameworkCore;
using MyStore.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MyStore.Data
{
    public class FruitStoreContext : DbContext
    {
        public FruitStoreContext(DbContextOptions<FruitStoreContext> options) : base(options) { }
        public DbSet<Fruit> Fruits { get; set; }
       
        public DbSet<Sale> Sales { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fruit>()
                .Property(f => f.Price)
                .HasPrecision(18, 2);
        }
    }
}
