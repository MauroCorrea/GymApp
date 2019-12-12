using System;
namespace GymTest.Models
{
    public class AppSettings
    {
        public Logging Logging { get; set; }

        public string AllowedHosts { get; set; }

        public string EmailConfiguration_Host { get; set; }
        public string EmailConfiguration_Port { get; set; }
        public string EmailConfiguration_Username { get; set; }
        public string EmailConfiguration_Password { get; set; }
        public string EmailConfiguration_BlindCopy { get; set; }

        public string PaymentNotificationDayToPay { get; set; }
        public string PaymentNotificationDayToPayMessage { get; set; }
        public string PaymentNotificationByDate { get; set; }
        public string PaymentNotificationByExpiration { get; set; }
        public string PaymentNotificationDaysBefore { get; set; }
        public string PaymentNotificationAssitanceBefore { get; set; }

        public MySQLConfiguration MySQLConfiguration { get; set; }

        public string TemplateEmailPath { get; set; }

        public string AlwaysRememberUser { get; set; }

        public string EnableAdminRegister { get; set; }
        public string EmailAdminRegistration { get; set; }

        public string AssistanceConfiguration_DiffHours { get; set; }
        public string AssistanceConfiguration_DiffMins { get; set; }
        public string AssistanceConfiguration_DiffSecs { get; set; }

        public string Client { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel;
    }

    public class MySQLConfiguration
    {
        public string ConnectionString { get; set; }
        public string Version { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
    }

}
