using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;

namespace GymTest.Controllers
{
    public class ScheduleMassivelyController : Controller
    {
        private readonly GymTestContext _context;

        public ScheduleMassivelyController(GymTestContext context)
        {
            _context = context;
        }

        // GET: ScheduleMassively/Create
        public IActionResult Create()
        {
            ViewData["DisciplineId"] = new SelectList(_context.Discipline, "DisciplineId", "DisciplineDescription");
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName");
            return View();
        }

        // POST: ScheduleMassively/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleMassivelyId,DisciplineId,StartTime,EndTime,ResourceId,Places,DataFormatStartString,DataFormatEndString")] ScheduleMassively scheduleMassively)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(scheduleMassively);
                CreateSingleScheduleClass(scheduleMassively);
                await _context.SaveChangesAsync();
                return Redirect("../Schedule/Index");//RedirectToAction(nameof(Index));
            }

            return Redirect("../Schedule/Index");//View(scheduleMassively);
        }

        private void CreateSingleScheduleClass(ScheduleMassively scheduleMassively)
        {
            var datesForSchedule = GetBeteweenDates(scheduleMassively.DataFormatStartString, scheduleMassively.DataFormatEndString);

            foreach (var date in datesForSchedule)
            {
                var scheduleClass = new Schedule
                {
                    Discipline = scheduleMassively.Discipline,
                    DisciplineId = scheduleMassively.DisciplineId,
                    EndTime = scheduleMassively.EndTime,
                    Places = scheduleMassively.Places,
                    Resource = scheduleMassively.Resource,
                    ResourceId = scheduleMassively.ResourceId,
                    StartTime = scheduleMassively.StartTime,
                    ScheduleDate = date
                };

                _context.Schedule.Add(scheduleClass);
            }
        }

        private List<DateTime> GetBeteweenDates(DateTime start, DateTime end)
        {
            var dates = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                .Select(offset => start.AddDays(offset))
                .Where(date => date.DayOfWeek == start.DayOfWeek)
                .ToList();

            return dates;
        }
    }
}
