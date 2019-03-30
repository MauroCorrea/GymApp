using Microsoft.EntityFrameworkCore;

namespace GymTest.Data
{
    public class GymTestContext : DbContext
    {
        public GymTestContext(DbContextOptions<GymTestContext> options)
            : base(options)
        {
        }

        public DbSet<GymTest.Models.User> User { get; set; }

        public DbSet<GymTest.Models.Payment> Payment { get; set; }

        public DbSet<GymTest.Models.Assistance> Assistance { get; set; }

        public DbSet<GymTest.Models.CashCategory> CashCategory { get; set; }

        public DbSet<GymTest.Models.CashMovement> CashMovement { get; set; }

        public DbSet<GymTest.Models.CashMovementType> CashMovementType { get; set; }

        public DbSet<GymTest.Models.MovementType> MovementType { get; set; }

        public DbSet<GymTest.Models.Supplier> Supplier { get; set; }

        public DbSet<GymTest.Models.MedicalEmergency> MedicalEmergency { get; set; }

        public DbSet<GymTest.Models.Notification> Notification { get; set; }

        public DbSet<GymTest.Models.Resource> Resource { get; set; }

        public DbSet<GymTest.Models.Role> Role { get; set; }

        public DbSet<GymTest.Models.Workday> Workday { get; set; }
    }
}
