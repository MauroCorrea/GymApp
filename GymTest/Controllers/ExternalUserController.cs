using System;
using GymTest.Data;
using GymTest.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using GymTest.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymTest.Controllers
{

    public class SchedulesCombo : IComparable<SchedulesCombo>
    {
        public string text { get; set; }
        public int scheduleId { get; set; }

        public int CompareTo(SchedulesCombo other)
        {
            if (string.Compare(this.text, other.text, StringComparison.InvariantCultureIgnoreCase) != 0)
                return string.Compare(this.text, other.text, StringComparison.InvariantCultureIgnoreCase);
            return 0;
        }
    }

    public class ExternalUserController : Controller
    {
        private readonly GymTestContext _context;

        private readonly IScheduleLogic _scheduleLogic;

        public ExternalUserController(GymTestContext context, IScheduleLogic scheduleLogic)
        {
            _context = context;
            _scheduleLogic = scheduleLogic;
        }

        // GET: Register
        public IActionResult Index()
        {
            var selectLists = createScheduleList();

            ViewData["Schedules"] = new SelectList(selectLists, "scheduleId", "text");
            return View("RegisterUser");
        }

        [HttpPost]
        public IActionResult Register([Bind("Token,DocumentNumber,ScheduleId")] RegisterUser registerUser)
        {
            if (ModelState.IsValid)
            {
                var registered = false;

                var user = _context.User.Where(u => u.DocumentNumber.Equals(registerUser.DocumentNumber.Trim()) &&
                                                    u.Token.Equals(registerUser.Token.Trim()));

                var registeredInfo = new AssistanceInformation();

                if (user != null && user.Count() == 1)
                {
                    registered = _scheduleLogic.RegisterUser(user.First().UserId, registerUser.ScheduleId);

                    registeredInfo.User = user.First();

                    if (registered)
                    {
                        registeredInfo.Message = "Registro exitoso.";
                        registeredInfo.AdditionalData = "Disfrute de su reserva";
                    }
                    else
                    {
                        registeredInfo.Message = "Registro fallido.";
                        registeredInfo.AdditionalData = "Su reserva no pudo realizarse. Contáctese con el gimnasio.";
                    }

                }
                else
                {
                    registeredInfo.Message = "Registro fallido.";
                    registeredInfo.AdditionalData = "No se encontró usuario con esos datos.";
                }

                return View("RegisterInformation", registeredInfo);
            }

            var selectLists = createScheduleList();
            ViewData["Schedules"] = new SelectList(selectLists, "scheduleId", "text");

            return View("RegisterUser", registerUser);
        }

        private List<SchedulesCombo> createScheduleList()
        {

            var schedules = from u
                           in _context.Schedule.Include(p => p.Discipline)
                           .Include(p => p.ScheduleUsers)
                            select u;

            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now.AddDays(7).Date;

            schedules = schedules.Where(s => s.ScheduleDate >= fromDate
                                        && s.ScheduleDate <= toDate);

            //schedules = schedules.OrderBy(d => d.Discipline.DisciplineDescription);

            List<SchedulesCombo> showSchedules = new List<SchedulesCombo>();

            foreach (var sche in schedules)
            {
                //Si quedan cupos disponibles.
                if (sche.ScheduleUsers != null && sche.ScheduleUsers.Count < sche.Places)
                {
                    var showSchedule = new SchedulesCombo();

                    showSchedule.text = sche.Discipline.DisciplineDescription + " - " +
                                        sche.ScheduleDate.ToShortDateString() + " - " +
                                        sche.StartTime + " - " +
                                        sche.EndTime;
                    showSchedule.scheduleId = sche.ScheduleId;

                    showSchedules.Add(showSchedule);
                }
            }

            showSchedules.Sort();

            return showSchedules;
        }
    }
}
