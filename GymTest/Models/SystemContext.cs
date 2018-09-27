using Microsoft.EntityFrameworkCore;

namespace GymTest.Models
{
    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options)
            : base(options)
        {
        }

        public DbSet<GymTest.Models.CashCategory> CashCategories { get; set; }

        public DbSet<GymTest.Models.CashMovement> CashMovements { get; set; }

        public DbSet<GymTest.Models.CashMovementType> CashMovementTypes { get; set; }

        public DbSet<GymTest.Models.Payment> Payments { get; set; }

        public DbSet<GymTest.Models.User> Users { get; set; }
    }
}
