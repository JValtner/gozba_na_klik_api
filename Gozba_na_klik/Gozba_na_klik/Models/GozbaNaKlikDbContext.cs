using Gozba_na_klik.Models.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Models
{
    public class GozbaNaKlikDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public GozbaNaKlikDbContext(DbContextOptions<GozbaNaKlikDbContext> options)
            : base(options) { }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealAddon> MealAddons { get; set; }
        public DbSet<Alergen> Alergens { get; set; }
        public DbSet<ClosedDate> ClosedDates { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<DeliveryPersonSchedule> DeliveryPersonSchedules { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserAlergen> UserAlergens { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasIndex(u => u.NormalizedEmail)
                .IsUnique();

            // Map Identity tables
            modelBuilder.Entity<IdentityRole<int>>(b => b.ToTable("Roles"));
            modelBuilder.Entity<IdentityUserRole<int>>(b => b.ToTable("UserRoles"));
            modelBuilder.Entity<IdentityUserClaim<int>>(b => b.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityUserLogin<int>>(b => b.ToTable("UserLogins"));
            modelBuilder.Entity<IdentityUserToken<int>>(b => b.ToTable("UserTokens"));
            modelBuilder.Entity<IdentityRoleClaim<int>>(b => b.ToTable("RoleClaims"));

            base.OnModelCreating(modelBuilder);
            // --- UserAlergens many-to-many ---
            modelBuilder.Entity<UserAlergen>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAlergens)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAlergen>()
                .HasOne(ua => ua.Alergen)
                .WithMany(a => a.UserAlergens)
                .HasForeignKey(ua => ua.AlergenId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAlergen>()
                .HasIndex(ua => new { ua.UserId, ua.AlergenId })
                .IsUnique();

            // --- MealAlergens many-to-many ---
            modelBuilder.Entity<Meal>()
                .HasMany(m => m.Alergens)
                .WithMany(a => a.Meals)
                .UsingEntity(j => j.ToTable("MealAlergens"));

            // --- User - Restaurant (Employee) ---
            modelBuilder.Entity<User>()
                .HasOne(u => u.Restaurant)
                .WithMany(r => r.Employees)
                .HasForeignKey(u => u.RestaurantId)
                .OnDelete(DeleteBehavior.SetNull);

            // --- Restaurant Owner ---
            modelBuilder.Entity<Restaurant>()
                .HasOne(r => r.Owner)
                .WithMany()
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Active Order (1:1) ---
            modelBuilder.Entity<User>()
                .HasOne(u => u.ActiveOrder)
                .WithOne(o => o.DeliveryPerson)
                .HasForeignKey<User>(u => u.ActiveOrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // --- Addresses ---
            modelBuilder.Entity<User>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.DefaultAddress)
                .WithMany()
                .HasForeignKey(u => u.DefaultAddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // --- Orders ---
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Restaurant)
                .WithMany()
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // --- OrderItems ---
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Meal)
                .WithMany()
                .HasForeignKey(oi => oi.MealId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
