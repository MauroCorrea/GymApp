using System;
using GymTest.Data;
using System.Linq;
using GymTest.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace GymTest.Services
{
    public class ScheculeLogicImpl : IScheduleLogic
    {

        private readonly ISendEmail _sendEmail;

        private readonly GymTestContext _context;

        private readonly ILogger<IPaymentLogic> _logger;

        private readonly IPaymentLogic _paymentLogic;


        public ScheculeLogicImpl(GymTestContext context, ISendEmail sendEmail, ILogger<IPaymentLogic> logger, IPaymentLogic paymentLogic)
        {
            _logger = logger;
            _context = context;
            _sendEmail = sendEmail;
            _paymentLogic = paymentLogic;
        }


        public int GetSchedulePlaces(int scheduleId)
        {
            var calendars = from m in _context.Schedule
                            select m;

            calendars = calendars.Where(s => s.ScheduleId == scheduleId);

            if (calendars.Count() == 1)
            {
                /*if (calendars.First().ScheduleUsers == null)
                    return calendars.First().Places;
                return calendars.First().Places - calendars.First().ScheduleUsers.Count;*/
            }
            return 0;
        }

        public bool RegisterUser(int userId, int scheduleId)
        {
            try
            {
                if (GetSchedulePlaces(scheduleId) > 0 && _paymentLogic.HasPaymentValid(userId))
                {
                    /*var calendar = _context.Schedule
                                    .Include(p => p.ScheduleUsers)
                                    .Single(c => c.ScheduleId == scheduleId);

                    var user = _context.User
                                    .AsNoTracking()
                                    .Single(c => c.UserId == userId);

                    calendar.ScheduleUsers.Add(new ScheduleUser
                    {
                        Schedule = calendar,
                        User = user
                    });

                    _context.SaveChanges();*/

                    return true;
                }
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error registering a user. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error registering a user. Detail: " + ex.InnerException.Message);
            }
            return false;
        }

        public void DeleteUserFromSchedule(int userId, int scheduleId)
        {
            try
            {
                /*var calendar = _context.Schedule
                                    .Include(p => p.ScheduleUsers)
                                    .Single(c => c.ScheduleId == scheduleId);

                var userToRemove = calendar.ScheduleUsers.Where(u => u.UserId == userId).First();
                calendar.ScheduleUsers.Remove(userToRemove);

                _context.SaveChanges();*/
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error deleting a user from schedule. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error deleting a user from schedule. Detail: " + ex.InnerException.Message);
            }
        }
    }
}
