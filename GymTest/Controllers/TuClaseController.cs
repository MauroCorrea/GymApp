using System;
using System.Collections.Generic;
using GymTest.Data;
using GymTest.Models;
using GymTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GymTest.Controllers
{
    public class ShowSchedules
    {
        public string Profesor { get; set; }

        public int ScheduleId { get; set; }

        public string Discipline { get; set; }

        public int DisciplineId { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int Cupos { get; set; }

        public DateTime ScheduleDate { get; set; }
    }

    public class TuClaseController : Controller
    {
        private readonly GymTestContext _context;

        private readonly IScheduleLogic _scheduleLogic;

        public TuClaseController(GymTestContext context, IScheduleLogic scheduleLogic)
        {
            _context = context;
            _scheduleLogic = scheduleLogic;
        }

        public List<ShowSchedules> GetSchedules(DateTime FromDate, DateTime ToDate, int disciplineId)
        {

            var schedules = from u
                           in _context.Schedule.Include(p => p.Discipline)
                                              .Include(p => p.Resource)
                                              .Include(p => p.ScheduleUsers)
                            select u;


            if (FromDate == DateTime.MinValue)
                FromDate = DateTime.Now.Date;

            schedules = schedules.Where(s => s.ScheduleDate >= FromDate);

            if (ToDate == DateTime.MinValue)
                ToDate = DateTime.Now.AddDays(7).Date;

            schedules = schedules.Where(s => s.ScheduleDate <= ToDate);


            if (disciplineId > 0)
                schedules = schedules.Where(s => s.DisciplineId == disciplineId);

            return convertFromSchedule(schedules.ToList());
        }

        private List<ShowSchedules> convertFromSchedule(List<Schedule> schedules)
        {
            List<ShowSchedules> showSchedules = new List<ShowSchedules>();

            foreach (var sche in schedules)
            {
                var showSchedule = new ShowSchedules();

                if (sche.ScheduleUsers != null)
                    showSchedule.Cupos = sche.Places - sche.ScheduleUsers.Count();

                showSchedule.Discipline = sche.Discipline.DisciplineDescription;
                showSchedule.DisciplineId = sche.DisciplineId;
                showSchedule.EndTime = sche.EndTime;
                showSchedule.Profesor = sche.Resource.FullName;
                showSchedule.ScheduleDate = sche.ScheduleDate;
                showSchedule.ScheduleId = sche.ScheduleId;
                showSchedule.StartTime = sche.StartTime;

                showSchedules.Add(showSchedule);
            }


            return showSchedules;
        }

        public List<Discipline> GetDisciplines()
        {
            return _context.Discipline.ToList();
        }

        public bool RegisterUser(string userId, int scheduleId)
        {
            var user = _context.User.Where(u => u.DocumentNumber.Equals(userId));

            if (user != null && user.Count() > 0)
                return _scheduleLogic.RegisterUser(user.First().UserId, scheduleId);

            return false;
        }

        public bool DeleteRegisterUser(int userId, int scheduleId)
        {
            _scheduleLogic.DeleteUserFromSchedule(userId, scheduleId);
            return true;
        }
    }
}
