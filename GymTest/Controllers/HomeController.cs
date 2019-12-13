using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GymTest.Models;
using GymTest.Services;
using GymTest.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace GymTest.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly GymTestContext _context;

        private readonly IAssistanceLogic _assistanceLogic;

        private readonly IPaymentNotificationLogic _paymentNotLogic;

        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        public HomeController(GymTestContext context, IAssistanceLogic assistanceLogic, IOptionsSnapshot<AppSettings> app, IPaymentNotificationLogic paymentNotiLogic)
        {
            _context = context;
            _assistanceLogic = assistanceLogic;
            _paymentNotLogic = paymentNotiLogic;
            _appSettings = app;
        }

        public IActionResult Index()
        {
            _paymentNotLogic.NotifyUsers();

            var dayOfMonth = _appSettings.Value.PaymentNotificationDayToPay;

            if (dayOfMonth != null && Convert.ToInt16(dayOfMonth) > 0)
            {
                //TODO: LA LOGICA TIENE QUE IR EN EL SERVICE. No es para todos, solo para los que no tienen un pago valido.
                //AutomaticProcess nextDayAutomaticSendMailProcess = _context.AutomaticProcess.Where(x => x.LastProcessDate == null).FirstOrDefault();
                //if (nextDayAutomaticSendMailProcess != null)
                //{
                //    DateTime realDateToSendMail = nextDayAutomaticSendMailProcess.NextProcessDate;
                //    if (Convert.ToInt16(dayOfMonth) >= realDateToSendMail.Day)
                //    {
                //        Notification notification = new Notification
                //        {
                //            Everyone = true,
                //            Message = _appSettings.Value.PaymentNotificationDayToPayMessage,
                //            Send = false,
                //            To = null
                //        };
                //        nextDayAutomaticSendMailProcess.LastProcessDate = DateTime.Now;

                //        _context.AutomaticProcess.Update(nextDayAutomaticSendMailProcess);
                //        _context.Notification.Add(notification);
                //        _context.SaveChanges();

                //        ViewBag.Articles = true;
                //    }
                //    else
                //    {
                //        ViewBag.Articles = false;
                //    }
                //}
            }
            else
            {
                ViewBag.Articles = false;
            }


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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public bool getTrue()
        {
            return true;
        }
    }
}
