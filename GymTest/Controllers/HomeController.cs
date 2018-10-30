using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GymTest.Models;
using GymTest.Controllers;
using GymTest.Services;
using GymTest.Data;

namespace GymTest.Controllers
{
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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact(string fingerprint)
        {

            if (_assistanceLogic.ProcessAssistance(fingerprint))
            {
                ViewData["Message"] = "Lo Encontramos!!";
                ViewData["ImgCondition"] = "OK";

                _assistanceLogic.ProcessAssistanceNotification(fingerprint);

            }
            else
            {
                ViewData["Message"] = "No lo encontramos :(";
                ViewData["ImgCondition"] = "PAY";
            }

            return View();
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
