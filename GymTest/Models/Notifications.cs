using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTest.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
            
        public bool Everyone { get; set; }

        public bool Send { get; set; }

        public string To { get; set; }

        public string Message { get; set; }
    }
}
