using System;
using GymTest.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GymTest.Services
{
    public class TimeZoneLogicImpl : ITimezoneLogic
    {
        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        private readonly ILogger<IPaymentLogic> _logger;

        public TimeZoneLogicImpl(ILogger<IPaymentLogic> logger, IOptionsSnapshot<AppSettings> app)
        {
            _appSettings = app;
            _logger = logger;
        }

        public DateTime GetCurrentDateTime(DateTime dateTime)
        {
            try
            {
                var info = TimeZoneInfo.FindSystemTimeZoneById(_appSettings.Value.TimeZone);

                DateTimeOffset localServerTime = DateTimeOffset.Now;
                DateTimeOffset usersTime = TimeZoneInfo.ConvertTime(localServerTime, info);
                return usersTime.DateTime;
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error getting datetime. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error getting datetime. Detail: " + ex.InnerException.Message);
            }
            return DateTime.Now;
        }
    }
}
