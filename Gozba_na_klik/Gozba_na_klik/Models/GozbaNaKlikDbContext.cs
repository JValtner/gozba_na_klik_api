using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Models
{
    public class GozbaNaKlikDbContext : DbContext
    {
        public GozbaNaKlikDbContext(DbContextOptions<GozbaNaKlikDbContext> options) : base(options) {}
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Predefined Admin accounts ---
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "Josip_admin", Password = "pass_jv", Email = "josipvaltner@gmail.com", Role = "Admin", UserImage = null },
                new User { Id = 2, Username = "Luka_admin", Password = "pass_lk", Email = "lukakovacevic@gmail.com", Role = "Admin", UserImage = null },
                new User { Id = 3, Username = "Boris_admin", Password = "pass_bl", Email = "borislaketic@gmail.com", Role = "Admin", UserImage = null },
                new User { Id = 4, Username = "Tamas_admin", Password = "pass_kt", Email = "kopasztamas@gmail.com", Role = "Admin", UserImage = null },
                new User { Id = 5, Username = "Uros_admin", Password = "pass_um", Email = "urosmilinovic@gmail.com", Role = "Admin", UserImage = null }
            );
        }
    }
}
