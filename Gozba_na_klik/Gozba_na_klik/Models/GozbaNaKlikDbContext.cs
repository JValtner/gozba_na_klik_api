using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Models
{
    public class GozbaNaKlikDbContext : DbContext
    {
        public GozbaNaKlikDbContext(DbContextOptions<GozbaNaKlikDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealAddon> MealAddons { get; set; }
        public DbSet<Alergen> Alergens { get; set; }
        public DbSet<ClosedDate> ClosedDates { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<DeliveryPersonSchedule> DeliveryPersonSchedules { get; set; }
        public DbSet<UserAlergen> UserAlergens { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Many-to-Many Relationship between Users and Alergens ---
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

            // Sprecava duplikate i zadrzava Id kao primarni kljuc
            modelBuilder.Entity<UserAlergen>()
                .HasIndex(ua => new { ua.UserId, ua.AlergenId })
                .IsUnique();

            // --- Many-to-Many Relationship between Meals and Alergens ---
            modelBuilder.Entity<Meal>()
            .HasMany(m => m.Alergens)
            .WithMany(a => a.Meals)
            .UsingEntity(j =>
            {
                j.ToTable("MealAlergens");
                j.HasData(
                    // Meal 1
                    new { MealsId = 1, AlergensId = 1 }, // Gluten
                    new { MealsId = 1, AlergensId = 2 }, // Eggs
                    new { MealsId = 1, AlergensId = 3 }, // Milk

                    // Meal 2
                    new { MealsId = 2, AlergensId = 1 }, // Gluten
                    new { MealsId = 2, AlergensId = 3 }, // Milk

                    // Meal 3
                    new { MealsId = 3, AlergensId = 2 }, // Eggs
                    new { MealsId = 3, AlergensId = 3 }, // Milk

                    // Meal 4
                    new { MealsId = 4, AlergensId = 4 }, // Fish

                    // Meal 5
                    new { MealsId = 5, AlergensId = 5 }, // Soy
                    new { MealsId = 5, AlergensId = 6 }, // Crustaceans
                    new { MealsId = 5, AlergensId = 7 }, // Sesame

                    // Meal 7
                    new { MealsId = 7, AlergensId = 5 }, // Soy

                    // Meal 8
                    new { MealsId = 8, AlergensId = 5 }, // Soy
                    new { MealsId = 8, AlergensId = 8 }, // Mustard

                    // Meal 9
                    new { MealsId = 9, AlergensId = 3 }, // Milk
                    new { MealsId = 9, AlergensId = 5 }, // Soy

                    // Meal 10
                    new { MealsId = 10, AlergensId = 1 } // Gluten
                );
            });

            // --- Predefined Users ---
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "Josip_admin", Password = "pass_jv", Email = "josipvaltner@gmail.com", Role = "Admin", UserImage = null, IsActive = true },
                new User { Id = 2, Username = "Luka_admin", Password = "pass_lk", Email = "lukakovacevic@gmail.com", Role = "Admin", UserImage = null, IsActive = true },
                new User { Id = 3, Username = "Boris_admin", Password = "pass_bl", Email = "borislaketic@gmail.com", Role = "Admin", UserImage = null, IsActive = true },
                new User { Id = 4, Username = "Tamas_admin", Password = "pass_kt", Email = "kopasztamas@gmail.com", Role = "Admin", UserImage = null, IsActive = true },
                new User { Id = 5, Username = "Uros_admin", Password = "pass_um", Email = "urosmilinovic@gmail.com", Role = "Admin", UserImage = null, IsActive = true },
                new User { Id = 7, Username = "Milan_owner", Password = "pass_mo", Email = "milan.owner@example.com", Role = "RestaurantOwner", UserImage = null, IsActive = true },
                new User { Id = 8, Username = "Ana_owner", Password = "pass_ao", Email = "ana.owner@example.com", Role = "RestaurantOwner", UserImage = null, IsActive = true },
                new User { Id = 9, Username = "Ivan_owner", Password = "pass_io", Email = "ivan.owner@example.com", Role = "RestaurantOwner", UserImage = null, IsActive = true },
                new User { Id = 10, Username = "Petar_employee", Password = "pass_pe", Email = "petar.employee@example.com", Role = "RestaurantEmployee", RestaurantId = 1, UserImage = null, IsActive = true },
                new User { Id = 11, Username = "Marko_delivery", Password = "pass_md", Email = "marko.delivery@example.com", Role = "DeliveryPerson", RestaurantId = 1, UserImage = null, IsActive = true },
                new User { Id = 12, Username = "Ana_employee", Password = "pass_ae", Email = "ana.employee@example.com", Role = "RestaurantEmployee", RestaurantId = 2, UserImage = null, IsActive = true },
                new User { Id = 13, Username = "Jovan_delivery", Password = "pass_jd", Email = "jovan.delivery@example.com", Role = "DeliveryPerson", RestaurantId = 2, UserImage = null, IsActive = true },
                new User { Id = 14, Username = "Nikola_employee", Password = "pass_ne", Email = "nikola.employee@example.com", Role = "RestaurantEmployee", RestaurantId = 3, UserImage = null, IsActive = true },
                new User { Id = 15, Username = "Sara_employee", Password = "pass_se", Email = "sara.employee@example.com", Role = "RestaurantEmployee", RestaurantId = 1, UserImage = null, IsActive = false } // ← suspendovan
            );

            // --- Restaurants ---
            modelBuilder.Entity<Restaurant>().HasData(
                new Restaurant
                {
                    Id = 1,
                    Name = "Bella Italia",
                    PhotoUrl = "...",
                    OwnerId = 7,
                    Description = "Authentic Italian dishes made with fresh ingredients.",
                    Address = "Some address 1",
                    Phone = "123456",
                    CreatedAt = new DateTime(2025, 9, 28, 8, 0, 0, DateTimeKind.Utc)
                },
                new Restaurant
                {
                    Id = 2,
                    Name = "Sushi Master",
                    PhotoUrl = "...",
                    OwnerId = 8,
                    Description = "Authentic Japanese dishes made with fresh ingredients.",
                    Address = "Some address 2",
                    Phone = "234567",
                    CreatedAt = new DateTime(2025, 9, 28, 8, 30, 0, DateTimeKind.Utc)
                },
                new Restaurant
                {
                    Id = 3,
                    Name = "Grill House",
                    PhotoUrl = "...",
                    OwnerId = 9,
                    Description = "Authentic Ausie dishes made with fresh ingredients.",
                    Address = "Some address 3",
                    Phone = "345678",
                    CreatedAt = new DateTime(2025, 9, 28, 9, 0, 0, DateTimeKind.Utc)
                }
            );
            modelBuilder.Entity<Meal>().HasData(
                // --- Bella Italia (RestaurantId = 1) ---
                new Meal { Id = 1, Name = "Spaghetti Carbonara", Description = "Classic Italian pasta with pancetta, egg, and pecorino cheese.", Price = 950, ImagePath = "...", RestaurantId = 1 },
                new Meal { Id = 2, Name = "Margherita Pizza", Description = "Fresh mozzarella, tomato sauce, and basil on a wood-fired crust.", Price = 890, ImagePath = "...", RestaurantId = 1 },
                new Meal { Id = 3, Name = "Lasagna al Forno", Description = "Layered pasta with beef ragù, bechamel sauce, and parmesan.", Price = 1100, ImagePath = "...", RestaurantId = 1 },

                // --- Sushi Master (RestaurantId = 2) ---
                new Meal { Id = 4, Name = "Salmon Nigiri", Description = "Fresh salmon on seasoned rice, served with wasabi.", Price = 620, ImagePath = "...", RestaurantId = 2 },
                new Meal { Id = 5, Name = "California Roll", Description = "Crab, avocado, and cucumber rolled in sesame rice.", Price = 750, ImagePath = "...", RestaurantId = 2 },
                new Meal { Id = 6, Name = "Tuna Sashimi", Description = "Thinly sliced tuna served with soy sauce and wasabi.", Price = 980, ImagePath = "...", RestaurantId = 2 },
                new Meal { Id = 7, Name = "Ramen Bowl", Description = "Rich miso broth with noodles, egg, and pork slices.", Price = 1100, ImagePath = "...", RestaurantId = 2 },

                // --- Grill House (RestaurantId = 3) ---
                new Meal { Id = 8, Name = "BBQ Ribs", Description = "Slow-cooked ribs with tangy BBQ sauce.", Price = 1450, ImagePath = "...", RestaurantId = 3 },
                new Meal { Id = 9, Name = "Grilled Chicken Breast", Description = "Juicy grilled chicken with seasonal vegetables.", Price = 980, ImagePath = "...", RestaurantId = 3 },
                new Meal { Id = 10, Name = "Beef Burger", Description = "Classic beef burger with cheddar, lettuce, and tomato.", Price = 890, ImagePath = "...", RestaurantId = 3 }
            );
            // ---------------------------
            // Seed Addons (20 total)
            // ---------------------------
            modelBuilder.Entity<MealAddon>().HasData(
                // MealId 1
                new MealAddon { Id = 2, Name = "Garlic Bread", Price = 150, Type = "independent", MealId = 1, IsActive = false },
                new MealAddon { Id = 4, Name = "Extra Sauce", Price = 100, Type = "chosen", MealId = 1, IsActive = true }, // active chosen addon

                // MealId 2
                new MealAddon { Id = 1, Name = "Extra Cheese", Price = 120, Type = "chosen", MealId = 2, IsActive = true },
                new MealAddon { Id = 5, Name = "Chili Flakes", Price = 50, Type = "independent", MealId = 2, IsActive = false },

                // MealId 3
                new MealAddon { Id = 3, Name = "Parmesan", Price = 80, Type = "chosen", MealId = 3, IsActive = true },

                // MealId 4
                new MealAddon { Id = 6, Name = "Soy Sauce", Price = 60, Type = "independent", MealId = 4, IsActive = false },
                new MealAddon { Id = 17, Name = "Teriyaki Sauce", Price = 90, Type = "chosen", MealId = 4, IsActive = true },

                // MealId 5
                new MealAddon { Id = 7, Name = "Extra Wasabi", Price = 70, Type = "chosen", MealId = 5, IsActive = false },
                new MealAddon { Id = 16, Name = "Spicy Mayo", Price = 80, Type = "chosen", MealId = 5, IsActive = true },

                // MealId 6
                new MealAddon { Id = 8, Name = "Ginger", Price = 50, Type = "independent", MealId = 6, IsActive = false },

                // MealId 7
                new MealAddon { Id = 9, Name = "Boiled Egg", Price = 120, Type = "chosen", MealId = 7, IsActive = false },
                new MealAddon { Id = 10, Name = "Extra Pork", Price = 200, Type = "chosen", MealId = 7, IsActive = true },
                new MealAddon { Id = 18, Name = "Extra Noodles", Price = 140, Type = "chosen", MealId = 7, IsActive = false },

                // MealId 8
                new MealAddon { Id = 13, Name = "BBQ Sauce", Price = 90, Type = "chosen", MealId = 8, IsActive = true },
                new MealAddon { Id = 14, Name = "Coleslaw", Price = 130, Type = "independent", MealId = 8, IsActive = false },

                // MealId 9
                new MealAddon { Id = 15, Name = "Grilled Vegetables", Price = 150, Type = "independent", MealId = 9, IsActive = false },
                new MealAddon { Id = 20, Name = "Caesar Salad", Price = 190, Type = "independent", MealId = 9, IsActive = false },

                // MealId 10
                new MealAddon { Id = 11, Name = "Fries", Price = 180, Type = "independent", MealId = 10, IsActive = false },
                new MealAddon { Id = 12, Name = "Onion Rings", Price = 160, Type = "independent", MealId = 10, IsActive = false },
                new MealAddon { Id = 19, Name = "Extra Beef", Price = 220, Type = "chosen", MealId = 10, IsActive = true }
            );


            // ---------------------------
            // Seed Alergens (8 total)
            // ---------------------------
            modelBuilder.Entity<Alergen>().HasData(
                new Alergen { Id = 1, Name = "Gluten" },
                new Alergen { Id = 2, Name = "Eggs" },
                new Alergen { Id = 3, Name = "Milk" },
                new Alergen { Id = 4, Name = "Fish" },
                new Alergen { Id = 5, Name = "Soy" },
                new Alergen { Id = 6, Name = "Crustaceans" },
                new Alergen { Id = 7, Name = "Sesame" },
                new Alergen { Id = 8, Name = "Mustard" }
            );


            modelBuilder.Entity<WorkSchedule>().HasData(
                new WorkSchedule { Id = 1, DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(21, 0, 0), RestaurantId = 1 },
                new WorkSchedule { Id = 2, DayOfWeek = DayOfWeek.Tuesday, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(21, 0, 0), RestaurantId = 1 },
                new WorkSchedule { Id = 3, DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeSpan(11, 0, 0), CloseTime = new TimeSpan(22, 0, 0), RestaurantId = 2 },
                new WorkSchedule { Id = 4, DayOfWeek = DayOfWeek.Tuesday, OpenTime = new TimeSpan(11, 0, 0), CloseTime = new TimeSpan(22, 0, 0), RestaurantId = 2 },
                new WorkSchedule { Id = 5, DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeSpan(10, 0, 0), CloseTime = new TimeSpan(20, 0, 0), RestaurantId = 3 },
                new WorkSchedule { Id = 6, DayOfWeek = DayOfWeek.Tuesday, OpenTime = new TimeSpan(10, 0, 0), CloseTime = new TimeSpan(20, 0, 0), RestaurantId = 3 }
            );

            // --- Closed Dates ---
            modelBuilder.Entity<ClosedDate>().HasData(
                new ClosedDate { Id = 1, Date = new DateTime(2025, 12, 25, 0, 0, 0, DateTimeKind.Utc), RestaurantId = 1, Reason = "Christmas" },
                new ClosedDate { Id = 2, Date = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), RestaurantId = 2, Reason = "New Year" },
                new ClosedDate { Id = 3, Date = new DateTime(2025, 7, 4, 0, 0, 0, DateTimeKind.Utc), RestaurantId = 3, Reason = "Independence Day" }
            );

            // Konfiguracija povezanosti izmedjuu User i Restaurant:
            // - Jedan restoran moze imati vise zaposlenih (One-to-Many)
            // - RestaurantId u User tabeli je foreign key (nullable)
            // - Kada se restoran obrise, RestaurantId u User ce biti NULL (zaposleni ostaju u sistemu, u slucaju da vlasnik zatvara jedan restoran, otvara drugi npr)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Restaurant)
                .WithMany(r => r.Employees)
                .HasForeignKey(u => u.RestaurantId)
                .OnDelete(DeleteBehavior.SetNull);

            // Konfiguracija povezanosti Owner-Restaurant (jedan vlasnik, vise restorana)
            // - Restaurant.Owner navigation property koristi Restaurant.OwnerId kao foreign key
            // - Ne dozvoljava brisanje vlasnika ako ima restorane (Restrict)
            modelBuilder.Entity<Restaurant>()
                .HasOne(r => r.Owner)
                .WithMany()
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DeliveryPersonSchedule>()
                .HasOne(s => s.DeliveryPerson)
                .WithMany()
                .HasForeignKey(s => s.DeliveryPersonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
