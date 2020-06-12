using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;
using Microsoft.AspNetCore.Authorization;
using GymTest.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GymTest.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly GymTestContext _context;

        private readonly IScheduleLogic _scheduleLogic;

        private readonly IPaymentLogic _paymentLogic;

        private UserManager<IdentityUser> _userManager;

        private readonly ITimezoneLogic _timeZone;

        private readonly ILogger<IPaymentLogic> _logger;

        private IHostingEnvironment _env;

        private readonly ISendEmail _sendEmail;

        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        public ScheduleController(GymTestContext context, IScheduleLogic scheduleLogic, IPaymentLogic paymentLogic, ITimezoneLogic timeZone, IHostingEnvironment env, ISendEmail sendEmail, IOptionsSnapshot<AppSettings> app, UserManager<IdentityUser> userManager, ILogger<IPaymentLogic> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _scheduleLogic = scheduleLogic;
            _paymentLogic = paymentLogic;
            _timeZone = timeZone;
            _env = env;
            _sendEmail = sendEmail;
            _appSettings = app;
        }

        public bool RegisterUser(int userId, int scheduleId)
        {
            return _scheduleLogic.RegisterUser(userId, scheduleId);
        }

        public int GetSchedulePlaces(int scheduleId)
        {
            return _scheduleLogic.GetSchedulePlaces(scheduleId);
        }

        // GET: Schedule
        public async Task<IActionResult> Index(DateTime FromDate, DateTime ToDate)
        {

            var schedules = from u
                           in _context.Schedule.Include(p => p.Discipline)
                                              .Include(p => p.Resource)
                            select u;

            if (FromDate == DateTime.MinValue)
            {
                FromDate = DateTime.Now.AddDays(1 - DateTime.Now.Day);
            }

            if (ToDate == DateTime.MinValue)
            {
                ToDate = DateTime.Now.AddDays(7);
            }

            schedules = schedules
                .Where(s => s.ScheduleDate >= FromDate && s.ScheduleDate <= ToDate)
                .OrderBy(s => s.ScheduleDate)
                .OrderBy(s => s.StartTime);

            return View(await schedules.ToListAsync());
        }

        // GET: Schedule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Discipline)
                .Include(s => s.Resource)
                .Include(s => s.ScheduleUsers)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);

            if (schedule.ScheduleUsers == null)
            {
                schedule.ScheduleUsers = new List<ScheduleUser>();
            }

            var userList = GetUserWithValidPayment(schedule.ScheduleUsers.ToList());

            if (schedule == null)
            {
                return NotFound();
            }

            if (schedule.ScheduleUsers == null)
                schedule.ScheduleUsers = new List<ScheduleUser>();

            var modelView = new ScheduleView()
            {
                Schedule = schedule,
                User = new SelectList(userList, "UserId", "FullName")
            };

            return View(modelView);
        }

        private List<User> GetUserWithValidPayment(List<ScheduleUser> addedUsers)
        {
            var result = new List<User>();

            var users = _context.User.ToList();
            foreach (var user in users)
            {
                var userIsIn = addedUsers.Where(u => u.UserId == user.UserId).FirstOrDefault() != null;
                if (!userIsIn && _paymentLogic.HasPaymentValid(user.UserId))
                {
                    result.Add(user);
                }
            }

            return result;
        }

        public async Task<IActionResult> InsertUserIntoScheduler(ScheduleView viewmodel)
        {
            if (viewmodel.Schedule.ScheduleId <= 0 || viewmodel.SelectedUser <= 0)
                return RedirectToAction("Details", new { id = viewmodel.Schedule.ScheduleId });

            var updated = _scheduleLogic.RegisterUser(viewmodel.SelectedUser, viewmodel.Schedule.ScheduleId);

            return RedirectToAction("Details", new { id = viewmodel.Schedule.ScheduleId });
        }


        public async Task<IActionResult> DeleteUserIntoScheduler(int idSchedule, int idUser)
        {

            _scheduleLogic.DeleteUserFromSchedule(idUser, idSchedule);
            return RedirectToAction("Details", new { id = idSchedule });
        }

        // GET: Schedule/Create
        public IActionResult Create()
        {
            ViewData["DisciplineId"] = new SelectList(_context.Discipline, "DisciplineId", "DisciplineDescription");
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName");
            return View();
        }

        // POST: Schedule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleId,DisciplineId,StartTime,EndTime,ResourceId,Places,ScheduleDate")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DisciplineId"] = new SelectList(_context.Discipline, "DisciplineId", "DisciplineDescription", schedule.DisciplineId);
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName", schedule.ResourceId);
            return View(schedule);
        }

        // GET: Schedule/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["DisciplineId"] = new SelectList(_context.Discipline, "DisciplineId", "DisciplineDescription", schedule.DisciplineId);
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName", schedule.ResourceId);
            return View(schedule);
        }

        // POST: Schedule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,DisciplineId,StartTime,EndTime,ResourceId,Places,ScheduleDate")] Schedule schedule)
        {
            if (id != schedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ScheduleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DisciplineId"] = new SelectList(_context.Discipline, "DisciplineId", "DisciplineDescription", schedule.DisciplineId);
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName", schedule.ResourceId);
            return View(schedule);
        }

        // GET: Schedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Discipline)
                .Include(s => s.Resource)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedule.FindAsync(id);
            _context.Schedule.Remove(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.ScheduleId == id);
        }

        public async Task<IActionResult> ExportToExcel(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (FromDate == DateTime.MinValue)
                    FromDate = _timeZone.GetCurrentDateTime(DateTime.Now);
                if (ToDate == DateTime.MinValue)
                    ToDate = _timeZone.GetCurrentDateTime(DateTime.Now).AddDays(7);


                string path = _env.WebRootPath;
                string Ruta_Publica_Excel = path + "/Agendas_" + _timeZone.GetCurrentDateTime(DateTime.Now).ToString("ddMMyyyyHHmmss") + ".xlsx";

                ExcelPackage Package = new ExcelPackage(new System.IO.FileInfo(Ruta_Publica_Excel));

                while (FromDate <= ToDate)
                {
                    //Creo la hoja para la fecha
                    var hoja = Package.Workbook.Worksheets.Add(FromDate.ToShortDateString());

                    //obtengo las clases para esa fecha
                    var schedules = from u
                               in _context.Schedule.Include(p => p.Resource)
                                                  .Include(p => p.Discipline)
                                                  .Include(s => s.ScheduleUsers)
                                    select u;

                    schedules = schedules
                        .Where(s => s.ScheduleDate == FromDate)
                        .OrderBy(s => s.StartTime)
                        .OrderBy(s => s.EndTime);

                    var rowNum = 1;
                    //armo la agenda para esa fecha
                    //Cabezal
                    hoja.Cells["A" + rowNum].Value = "Inicio";
                    hoja.Cells["B" + rowNum].Value = "Fin";
                    hoja.Cells["C" + rowNum].Value = "Disciplina";
                    hoja.Cells["D" + rowNum].Value = "Instructor";
                    hoja.Cells["E" + rowNum].Value = "Cupos";
                    hoja.Cells["F" + rowNum].Value = "Socios inscriptos";

                    hoja.Cells["A" + rowNum + ":F" + rowNum].Style.Font.Bold = true;
                    hoja.Cells["A" + rowNum + ":F" + rowNum].Style.Font.Size = 15;

                    //agenda
                    rowNum = 2;
                    foreach (var clase in schedules)
                    {
                        hoja.Cells["A" + rowNum].Value = clase.StartTime;
                        hoja.Cells["B" + rowNum].Value = clase.EndTime;
                        hoja.Cells["C" + rowNum].Value = clase.Discipline.DisciplineDescription;
                        hoja.Cells["D" + rowNum].Value = clase.Resource.FullName;
                        hoja.Cells["E" + rowNum].Value = clase.Places;
                        if (clase.ScheduleUsers != null)
                            hoja.Cells["F" + rowNum].Value = clase.ScheduleUsers.Count;
                        else
                            hoja.Cells["F" + rowNum].Value = 0;

                        rowNum++;
                    }

                    hoja.DefaultColWidth = 20;

                    FromDate = FromDate.AddDays(1);
                }

                Package.Save();


                #region send Mail
                var user = await _userManager.GetUserAsync(User);
                var userEmail = user.Email;
                var userName = user.UserName;
                if (string.IsNullOrEmpty(userEmail))
                    userEmail = _appSettings.Value.EmailConfiguration_Username;
                if (string.IsNullOrEmpty(userName))
                    userName = "Administrador";

                var bodyData = new Dictionary<string, string>
                {
                    { "UserName", userName },
                    { "Title", "Envío de Agenda." },
                    { "message", "Descargue el archivo adjunto." }
                };

                _sendEmail.SendEmail(bodyData,
                                     "ReportTemplate",
                                     "Reporte de agenda",
                                     new List<string>() { userEmail },
                                     new List<string>() { Ruta_Publica_Excel }
                                    );
                #endregion
                try
                {
                    if ((System.IO.File.Exists(Ruta_Publica_Excel)))
                    {
                        System.IO.File.Delete(Ruta_Publica_Excel);
                    }
                }
                catch (Exception ex)
                {
                    var messageError = ex.Message;
                    _logger.LogError("Error creating Agenda. Detail: " + messageError);
                    if (ex.InnerException != null)
                        _logger.LogError("Error creating Agenda. Detail: " + ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error creating Agenda. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error creating Agenda. Detail: " + ex.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
