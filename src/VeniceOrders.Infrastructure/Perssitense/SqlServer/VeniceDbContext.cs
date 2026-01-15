using Microsoft.EntityFrameworkCore;
using VeniceOrders.Domain.Entities;

namespace VeniceOrders.Infrastructure.Perssitense.SqlServer
{
    public class VeniceDbContext : DbContext
    {
        public VeniceDbContext(DbContextOptions<VeniceDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClienteId).IsRequired();
                entity.Property(e => e.Data).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });
        }
    }
}
