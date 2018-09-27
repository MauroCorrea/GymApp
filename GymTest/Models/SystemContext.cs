using Microsoft.EntityFrameworkCore;

namespace GymTest.Models
{
    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options)
            : base(options)
        {
        }

        public DbSet<MVCApp.Models.User> Users { get; set; }

        public DbSet<MVCApp.Models.CashCategory> CashCategories { get; set; }

        public DbSet<MVCApp.Models.CashMovement> CashMovements { get; set; }

        public DbSet<MVCApp.Models.Payment> Payments { get; set; }
    }
}
