using Microsoft.EntityFrameworkCore;
using SpecA.Models;

namespace SpecA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Department> Departments => Set<Department>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>()
                .HasMany(c => c.Departments)
                .WithOne(d => d.Company!)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Fast per-company listing.
            modelBuilder.Entity<Department>()
                .HasIndex(d => d.CompanyId);

            // Department name unique within its parent company (defense-in-depth;
            // primary enforcement is in the controller for friendly messages).
            modelBuilder.Entity<Department>()
                .HasIndex(d => new { d.CompanyId, d.Name })
                .IsUnique();
        }
    }
}
