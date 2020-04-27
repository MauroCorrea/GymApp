using Microsoft.EntityFrameworkCore;
using GymTest.Models;

namespace GymTest.Data
{
    public class GymTestContext : DbContext
    {
        public GymTestContext(DbContextOptions<GymTestContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScheduleUser>()
               .HasKey(x => new { x.ScheduleId, x.UserId });

            modelBuilder.Entity<ScheduleUser>()
                .HasOne(su => su.Schedule)
                .WithMany(su => su.ScheduleUsers)
                .HasForeignKey(su => su.ScheduleId);

            modelBuilder.Entity<ScheduleUser>()
                .HasOne(su => su.User)
                .WithMany(su => su.ScheduleUsers)
                .HasForeignKey(su => su.UserId);
        }

        public DbSet<GymTest.Models.User> User { get; set; }

        public DbSet<GymTest.Models.Payment> Payment { get; set; }

        public DbSet<GymTest.Models.Assistance> Assistance { get; set; }

        public DbSet<GymTest.Models.CashCategory> CashCategory { get; set; }

        public DbSet<GymTest.Models.CashSubcategory> CashSubcategory { get; set; }

        public DbSet<GymTest.Models.CashMovement> CashMovement { get; set; }

        public DbSet<GymTest.Models.CashMovementType> CashMovementType { get; set; }

        public DbSet<GymTest.Models.MovementType> MovementType { get; set; }

        public DbSet<GymTest.Models.Supplier> Supplier { get; set; }

        public DbSet<GymTest.Models.MedicalEmergency> MedicalEmergency { get; set; }

        public DbSet<GymTest.Models.Notification> Notification { get; set; }

        public DbSet<GymTest.Models.Resource> Resource { get; set; }

        public DbSet<GymTest.Models.Role> Role { get; set; }

        public DbSet<GymTest.Models.Workday> Workday { get; set; }

        public DbSet<GymTest.Models.PaymentMedia> PaymentMedia { get; set; }

        public DbSet<GymTest.Models.UserReport> UserReport { get; set; }

        public DbSet<GymTest.Models.AutomaticProcess> AutomaticProcess { get; set; }

        public DbSet<GymTest.Models.Discipline> Discipline { get; set; }

        public DbSet<GymTest.Models.Schedule> Schedule { get; set; }

        public DbSet<GymTest.Models.ScheduleUser> ScheduleUser { get; set; }

        public DbSet<GymTest.Models.ScheduleMassively> ScheduleMassively { get; set; }

    }
}
