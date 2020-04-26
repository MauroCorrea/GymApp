using System;
namespace GymTest.Models
{
    public class ScheduleUser
    {
        public ScheduleUser()
        {
        }

        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
