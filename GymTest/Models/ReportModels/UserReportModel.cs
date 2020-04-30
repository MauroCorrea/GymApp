using System;
namespace GymTest.Models.ReportModels
{
    public class UserReportModel
    {
        public string Name { get; set; }
        public DateTime? SingInDate { get; set; }
        public DateTime BirthDate { get; set; }
        public int AssistanceCount { get; set; }
        public DateTime AssistFrom { get; set; }
        public DateTime AssistTo { get; set; }
        public string paymentType { get; set; }
        public string paymentMedia { get; set; }
        public int PaymentCount { get; set; }
        public DateTime PaymentFrom { get; set; }
        public DateTime PaymentTo { get; set; }

        public UserReportModel()
        {
        }
    }
}
