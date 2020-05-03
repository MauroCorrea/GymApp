using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymTest.Models
{
    public class ScheduleView
    {
        public Schedule Schedule { get; set; }

        public SelectList User { get; set; }

        public int SelectedUser { get; set; }

        public ScheduleView()
        {
        }

    }
}
