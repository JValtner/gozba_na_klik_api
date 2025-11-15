using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(
            GozbaNaKlikDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            // --- 1. Roles ---
            string[] roles = { "Admin", "RestaurantOwner", "RestaurantEmployee", "DeliveryPerson", "User" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }

            // --- 2. Admins & Owners (no RestaurantId yet) ---
            var basicUsers = new[]
            {
                new { UserName = "Josip_admin", Email = "josipvaltner@gmail.com", Role = "Admin", IsActive = true },
                new { UserName = "Luka_admin", Email = "lukakovacevic@gmail.com", Role = "Admin", IsActive = true },
                new { UserName = "Boris_admin", Email = "borislaketic@gmail.com", Role = "Admin", IsActive = true },
                new { UserName = "Tamas_admin", Email = "kopasztamas@gmail.com", Role = "Admin", IsActive = true },
                new { UserName = "Uros_admin", Email = "urosmilinovic@gmail.com", Role = "Admin", IsActive = true },

                new { UserName = "Milan_owner", Email = "milan.owner@example.com", Role = "RestaurantOwner", IsActive = true },
                new { UserName = "Ana_owner", Email = "ana.owner@example.com", Role = "RestaurantOwner", IsActive = true },
                new { UserName = "Ivan_owner", Email = "ivan.owner@example.com", Role = "RestaurantOwner", IsActive = true }
            };

            foreach (var u in basicUsers)
            {
                if (await userManager.FindByNameAsync(u.UserName) == null)
                {
                    var user = new User
                    {
                        UserName = u.UserName,
                        Email = u.Email,
                        IsActive = u.IsActive,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var result = await userManager.CreateAsync(user, "Pass@123");
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(user, u.Role);
                }
            }

            // --- 3. Restaurants ---
            if (!await context.Restaurants.AnyAsync())
            {
                var milanOwner = await userManager.FindByNameAsync("Milan_owner");
                var anaOwner = await userManager.FindByNameAsync("Ana_owner");
                var ivanOwner = await userManager.FindByNameAsync("Ivan_owner");

                var restaurants = new[]
                {
                    new Restaurant
                    {
                        Name = "Bella Italia",
                        PhotoUrl = "...",
                        OwnerId = milanOwner.Id,
                        Description = "Authentic Italian dishes made with fresh ingredients.",
                        Address = "Some address 1",
                        Phone = "123456",
                        CreatedAt = new DateTime(2025, 9, 28, 8, 0, 0, DateTimeKind.Utc)
                    },
                    new Restaurant
                    {
                        Name = "Sushi Master",
                        PhotoUrl = "...",
                        OwnerId = anaOwner.Id,
                        Description = "Authentic Japanese dishes made with fresh ingredients.",
                        Address = "Some address 2",
                        Phone = "234567",
                        CreatedAt = new DateTime(2025, 9, 28, 8, 30, 0, DateTimeKind.Utc)
                    },
                    new Restaurant
                    {
                        Name = "Grill House",
                        PhotoUrl = "...",
                        OwnerId = ivanOwner.Id,
                        Description = "Authentic Ausie dishes made with fresh ingredients.",
                        Address = "Some address 3",
                        Phone = "345678",
                        CreatedAt = new DateTime(2025, 9, 28, 9, 0, 0, DateTimeKind.Utc)
                    }
                };

                context.Restaurants.AddRange(restaurants);
                await context.SaveChangesAsync();
            }

            // --- 4. Dependent Users (employees, delivery) ---
            var dependentUsers = new[]
            {
                new { UserName = "Petar_employee", Email = "petar.employee@example.com", Role = "RestaurantEmployee", IsActive = true, OwnerName = "Milan_owner" },
                new { UserName = "Ana_employee", Email = "ana.employee@example.com", Role = "RestaurantEmployee", IsActive = true, OwnerName = "Ana_owner" },
                new { UserName = "Nikola_employee", Email = "nikola.employee@example.com", Role = "RestaurantEmployee", IsActive = true, OwnerName = "Ivan_owner" },
                new { UserName = "Sara_employee", Email = "sara.employee@example.com", Role = "RestaurantEmployee", IsActive = false, OwnerName = "Milan_owner" },
                new { UserName = "Marko_delivery", Email = "marko.delivery@example.com", Role = "DeliveryPerson", IsActive = true, OwnerName = "Milan_owner" },
                new { UserName = "Jovan_delivery", Email = "jovan.delivery@example.com", Role = "DeliveryPerson", IsActive = true, OwnerName = "Ana_owner" }
            };

            foreach (var u in dependentUsers)
            {
                if (await userManager.FindByNameAsync(u.UserName) == null)
                {
                    var owner = await userManager.FindByNameAsync(u.OwnerName);
                    var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.OwnerId == owner.Id);

                    var user = new User
                    {
                        UserName = u.UserName,
                        Email = u.Email,
                        IsActive = u.IsActive,
                        RestaurantId = restaurant?.Id,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var result = await userManager.CreateAsync(user, "Pass@123");
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(user, u.Role);
                }
            }

            // --- 5. Meals (example) ---
            if (!await context.Meals.AnyAsync())
            {
                var meals = new[]
            {
                // Bella Italia (Italian)
                new Meal { Name = "Spaghetti Carbonara", Price = 950, RestaurantId = context.Restaurants.First(r => r.Name == "Bella Italia").Id, Description = "Classic Italian pasta with eggs, cheese, pancetta, and pepper." },
                new Meal { Name = "Margherita Pizza", Price = 890, RestaurantId = context.Restaurants.First(r => r.Name == "Bella Italia").Id, Description = "Traditional Italian pizza with tomato, mozzarella, and basil." },
                new Meal { Name = "Lasagna al Forno", Price = 1100, RestaurantId = context.Restaurants.First(r => r.Name == "Bella Italia").Id, Description = "Baked Italian lasagna with rich meat sauce and béchamel." },

                // Sushi Master (Japanese)
                new Meal { Name = "Salmon Nigiri", Price = 620, RestaurantId = context.Restaurants.First(r => r.Name == "Sushi Master").Id, Description = "Fresh salmon over seasoned rice, classic nigiri." },
                new Meal { Name = "California Roll", Price = 750, RestaurantId = context.Restaurants.First(r => r.Name == "Sushi Master").Id, Description = "Crab, avocado, and cucumber rolled in rice and seaweed." },
                new Meal { Name = "Tuna Sashimi", Price = 980, RestaurantId = context.Restaurants.First(r => r.Name == "Sushi Master").Id, Description = "Thinly sliced raw tuna, served with soy sauce and wasabi." },
                new Meal { Name = "Ramen Bowl", Price = 1100, RestaurantId = context.Restaurants.First(r => r.Name == "Sushi Master").Id, Description = "Japanese noodle soup with pork, egg, and vegetables." },

                // Grill House (Grill/American)
                new Meal { Name = "BBQ Ribs", Price = 1450, RestaurantId = context.Restaurants.First(r => r.Name == "Grill House").Id, Description = "Tender grilled ribs with smoky barbecue sauce." },
                new Meal { Name = "Grilled Chicken Breast", Price = 980, RestaurantId = context.Restaurants.First(r => r.Name == "Grill House").Id, Description = "Juicy grilled chicken breast with herbs and spices." },
                new Meal { Name = "Beef Burger", Price = 890, RestaurantId = context.Restaurants.First(r => r.Name == "Grill House").Id, Description = "Classic beef burger with lettuce, tomato, and cheese." }
            };


                context.Meals.AddRange(meals);
                await context.SaveChangesAsync();
            }

            Console.WriteLine("Seeding complete.");
        }
    }
}
