using System;
namespace GymTest.Models
{
    public class AssistanceInformation
    {
        public string AdditionalData { get; set; }

        public virtual User User { get; set; }

        public string Message { get; set; }

        public AssistanceInformation()
        {
        }
    }
}
