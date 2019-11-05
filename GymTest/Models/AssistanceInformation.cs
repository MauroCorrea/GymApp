using System;
using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
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
