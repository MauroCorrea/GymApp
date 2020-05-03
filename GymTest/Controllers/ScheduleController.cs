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

namespace GymTest.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly GymTestContext _context;

        private readonly IScheduleLogic _scheduleLogic;

        public ScheduleController(GymTestContext context, IScheduleLogic scheduleLogic)
        {
            _context = context;
            _scheduleLogic = scheduleLogic;
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
                FromDate = DateTime.Now.AddDays(-7).Date;

            schedules = schedules.Where(s => s.ScheduleDate >= FromDate);

            if (ToDate == DateTime.MinValue)
                ToDate = DateTime.Now.Date;

            schedules = schedules.Where(s => s.ScheduleDate <= ToDate);

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

            if(schedule.ScheduleUsers == null)
            {
                schedule.ScheduleUsers = new List<ScheduleUser>();
            }

            var userList = _context.User;

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

<<<<<<< HEAD
        public async Task<IActionResult> InsertuserIntoScheduler(int idSchedule, long idUser)
        {
            //return RedirectToAction(nameof(Edit));

            if (idSchedule <= 0 || idUser <= 0)
                return RedirectToAction(nameof(Details));
=======
        public async Task<IActionResult> InsertUserIntoScheduler(ScheduleView viewmodel)
        {
            if (viewmodel.Schedule.ScheduleId <= 0 || viewmodel.SelectedUser <= 0)
                return RedirectToAction("Details", new { id = viewmodel.Schedule.ScheduleId });
>>>>>>> 012e98580c7414a074a4aed5936c9a2d00b96711

            var updated = _scheduleLogic.RegisterUser(viewmodel.SelectedUser, viewmodel.Schedule.ScheduleId);

            if (updated)
                return RedirectToAction(nameof(Index));

            return RedirectToAction("Details", new { id = viewmodel.Schedule.ScheduleId });
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
    }
}
