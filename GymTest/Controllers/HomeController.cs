using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GymTest.Models;
using GymTest.Services;
using GymTest.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GymTest.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly GymTestContext _context;

        private readonly IAssistanceLogic _assistanceLogic;

        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        public HomeController(GymTestContext context, IAssistanceLogic assistanceLogic, IOptionsSnapshot<AppSettings> app)
        {
            _context = context;
            _assistanceLogic = assistanceLogic;
            _appSettings = app;
        }

        public IActionResult Index(DateTime? FromDate)
        {
            ViewBag.Articles = false;
            if (FromDate == null) FromDate = DateTime.Today;

            var scheduleElements = _context.Schedule.Where(s => s.ScheduleDate == FromDate).Include(s => s.Field).ToList();
            scheduleElements = scheduleElements.OrderBy(s => Convert.ToInt16(s.StartTime.Split(":")[0])).ThenBy(s => s.FieldId).ToList();

            return View(scheduleElements);
        }

        public void About(string fingerprint)
        {
            if (string.IsNullOrEmpty(fingerprint))
                fingerprint = string.Empty;

            if (Request.Form["1"] != "")
                fingerprint += 1;
        }

        public IActionResult Contact(string fingerprint)
        {
            var assistanceInfo = _assistanceLogic.ProcessAssistance(fingerprint);

            return View(assistanceInfo);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public bool getTrue()
        {
            return true;
        }
    }
}
