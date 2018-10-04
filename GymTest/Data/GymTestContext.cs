using Microsoft.EntityFrameworkCore;

namespace GymTest.Models
{
    public class GymTestContext : DbContext
    {
        public GymTestContext (DbContextOptions<GymTestContext> options)
            : base(options)
        {
        }

        public DbSet<GymTest.Models.User> User { get; set; }

        public DbSet<GymTest.Models.Payment> Payment { get; set; }

        public DbSet<GymTest.Models.Assistance> Assistance { get; set; }

        public DbSet<GymTest.Models.CashCategory> CashCategory { get; set; }

        public DbSet<GymTest.Models.CashMovement> CashMovement { get; set; }

        public DbSet<GymTest.Models.CashMovementType> CashMovementType { get; set; }
    }
}
