using System;
namespace GymTest.Models
{
    public class AutomaticProcess
    {
        public int AutomaticProcessId { get; set; }
        public string AutomaticProcessDesctipion { get; set; }
        public DateTime NextProcessDate { get; set; }

        public AutomaticProcess()
        {
        }
    }
}
