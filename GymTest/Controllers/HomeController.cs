using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GymTest.Models;
using GymTest.Services;
using GymTest.Data;
using Microsoft.AspNetCore.Authorization;

namespace GymTest.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly GymTestContext _context;

        private readonly IAssistanceLogic _assistanceLogic;

        public HomeController(GymTestContext context, IAssistanceLogic assistanceLogic)
        {
            _context = context;
            _assistanceLogic = assistanceLogic;
        }

        public IActionResult Index()
        {
            return View();
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
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public bool getTrue()
        {
            return true;
        }
    }
}
