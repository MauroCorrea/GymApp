using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class Notification
    {
        public int NotificationId { get; set; }

        public bool Everyone { get; set; }

        public bool Send { get; set; }

        public string To { get; set; }

        public string Message { get; set; }
    }
}
