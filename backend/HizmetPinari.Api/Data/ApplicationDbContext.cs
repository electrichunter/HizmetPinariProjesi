
using HizmetPinari.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HizmetPinari.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserRoles için birleşik birincil anahtar (composite primary key) tanımlaması
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserID, ur.RoleID });

            // User ve Role ile UserRole arasındaki ilişkileri tanımlama (many-to-many)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserID);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleID);

            // Customer ve ServiceRequest arasındaki ilişki (one-to-many)
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Customer)
                .WithMany(u => u.ServiceRequests)
                .HasForeignKey(sr => sr.CustomerID)
                .OnDelete(DeleteBehavior.Restrict); // Müşteri silinirse talepleri silinmesin

            // ServiceProvider ve Offer arasındaki ilişki (one-to-many)
            modelBuilder.Entity<Offer>()
                .HasOne(o => o.ServiceProvider)
                .WithMany(u => u.Offers)
                .HasForeignKey(o => o.ProviderID)
                .OnDelete(DeleteBehavior.Cascade); // Hizmet veren silinirse teklifleri de silinsin
        }
    }
}
