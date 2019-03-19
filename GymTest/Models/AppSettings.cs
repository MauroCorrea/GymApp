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

        public MySQLConfiguration MySQLConfiguration { get; set; }

        public string AssistanceConfiguration_DiffHours { get; set; }

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
