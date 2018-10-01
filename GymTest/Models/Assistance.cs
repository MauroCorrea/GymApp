using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymTest.Models
{
    public class Assistance
    {
        public int ID { get; set; }
        public virtual User User { get; set; }
        public DateTime AssistanceDate { get; set; }
    }
}
