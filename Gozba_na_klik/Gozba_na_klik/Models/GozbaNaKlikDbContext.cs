using Gozba_na_klik.Models.MealModels;
using Gozba_na_klik.Models.RestaurantModels;
using Gozba_na_klik.Models.Restaurants;
using Microsoft.EntityFrameworkCore;
using Gozba_na_klik.Models.Customers;

namespace Gozba_na_klik.Models
{
    public class GozbaNaKlikDbContext : DbContext
    {
        public GozbaNaKlikDbContext(DbContextOptions<GozbaNaKlikDbContext> options) : base(options) {}
        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public  DbSet<Meal> Meals { get; set; }
        public DbSet<MealAddon> MealAddons { get; set; }
        public DbSet<Alergen> Alergens { get; set; }
        public DbSet<ClosedDate> ClosedDates { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Predefined Admin accounts ---
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "Josip_admin", Password = "pass_jv", Email = "josipvaltner@gmail.com", Role = "Admin", UserImage = null },
                new User { Id = 2, Username = "Luka_admin", Password = "pass_lk", Email = "lukakovacevic@gmail.com", Role = "Admin", UserImage = null },
                new User { Id = 3, Username = "Boris_admin", Password = "pass_bl", Email = "borislaketic@gmail.com", Role = "Admin", UserImage = null },
                new User { Id = 4, Username = "Tamas_admin", Password = "pass_kt", Email = "kopasztamas@gmail.com", Role = "Admin", UserImage = null },
                new User { Id = 5, Username = "Uros_admin", Password = "pass_um", Email = "urosmilinovic@gmail.com", Role = "Admin", UserImage = null },
                new User { Id = 7, Username = "Milan_owner", Password = "pass_mo", Email = "milan.owner@example.com", Role = "RestaurantOwner", UserImage = null },
                new User { Id = 8, Username = "Ana_owner", Password = "pass_ao", Email = "ana.owner@example.com", Role = "RestaurantOwner", UserImage = null },
                new User { Id = 9, Username = "Ivan_owner", Password = "pass_io", Email = "ivan.owner@example.com", Role = "RestaurantOwner", UserImage = null }
            );

            modelBuilder.Entity<Restaurant>().HasData(
                new Restaurant { Id = 1, Name = "Bella Italia", PhotoUrl = "...", OwnerId = 7, Description = "Authentic Italian dishes made with fresh ingredients.", CreatedAt = new DateTime(2025, 9, 28, 8, 0, 0, DateTimeKind.Utc) },
                new Restaurant { Id = 2, Name = "Sushi Master", PhotoUrl = "...", OwnerId = 8, Description = "Authentic Japanese dishes made with fresh ingredients.", CreatedAt = new DateTime(2025, 9, 28, 8, 30, 0, DateTimeKind.Utc) },
                new Restaurant { Id = 3, Name = "Grill House", PhotoUrl = "...", OwnerId = 9, Description = "Authentic Ausie dishes made with fresh ingredients.", CreatedAt = new DateTime(2025, 9, 28, 9, 0, 0, DateTimeKind.Utc) }
            );


            modelBuilder.Entity<WorkSchedule>().HasData(
                new WorkSchedule { Id = 1, DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(21, 0, 0), RestaurantId = 1 },
                new WorkSchedule { Id = 2, DayOfWeek = DayOfWeek.Tuesday, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(21, 0, 0), RestaurantId = 1 },

                new WorkSchedule { Id = 3, DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeSpan(11, 0, 0), CloseTime = new TimeSpan(22, 0, 0), RestaurantId = 2 },
                new WorkSchedule { Id = 4, DayOfWeek = DayOfWeek.Tuesday, OpenTime = new TimeSpan(11, 0, 0), CloseTime = new TimeSpan(22, 0, 0), RestaurantId = 2 },

                new WorkSchedule { Id = 5, DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeSpan(10, 0, 0), CloseTime = new TimeSpan(20, 0, 0), RestaurantId = 3 },
                new WorkSchedule { Id = 6, DayOfWeek = DayOfWeek.Tuesday, OpenTime = new TimeSpan(10, 0, 0), CloseTime = new TimeSpan(20, 0, 0), RestaurantId = 3 }
            );

            modelBuilder.Entity<ClosedDate>().HasData(
                new ClosedDate { Id = 1, Date = new DateTime(2025, 12, 25, 0, 0, 0, DateTimeKind.Utc), RestaurantId = 1 },
                new ClosedDate { Id = 2, Date = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), RestaurantId = 2 },
                new ClosedDate { Id = 3, Date = new DateTime(2025, 7, 4, 0, 0, 0, DateTimeKind.Utc), RestaurantId = 3 }
            );

        }
    }
}
