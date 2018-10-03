using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GymTest.Models;
using GymTest.Controllers;

namespace GymTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly GymTestContext _context;

        public HomeController(GymTestContext context)
        {
            _context = context;
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
            var users = from m in _context.User
                        select m;

            users = users.Where(s => s.DocumentNumber.Equals(fingerprint));

            if (users.Count()== 1)
            {
                var ass = from a in _context.Assistance select a;
                ass = ass.Where(a => a.UserId.Equals(users.FirstOrDefault().UserId));

                ViewData["Message"] = "Lo Encontramos!! Y es la vez número " + (ass.Count() + 1) + " que vine!";
                Assistance assistance = new Assistance();
                assistance.User = users.FirstOrDefault();
                assistance.AssistanceDate = DateTime.Now;
                _context.Assistance.Add(assistance);
                _context.SaveChangesAsync();
            }
            else
            {
                ViewData["Message"] = "No lo encontramos :(";
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
    }
}
