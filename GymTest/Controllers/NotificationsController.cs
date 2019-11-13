using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;
using GymTest.Services;
using Microsoft.AspNetCore.Authorization;

namespace GymTest.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly GymTestContext _context;
        private readonly ISendEmail _sendEmail;

        public NotificationsController(GymTestContext context, ISendEmail sendEmail)
        {
            _context = context;
            _sendEmail = sendEmail;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            return View(await _context.Notification.ToListAsync());
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notification
                .FirstOrDefaultAsync(m => m.NotificationId == id);

            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //////[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificationId,Everyone,Send,To,Message")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);

                SendNotification(notification);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        private void SendNotification(Notification notification)
        {
            if (string.IsNullOrEmpty(notification.To))
            {
                notification.Everyone = true;
                var users = from u in _context.User select u;
                foreach (User user in users)
                {
                    SendMail(user.FullName, notification.Message, user.Email);
                }
            }
            else
            {
                notification.Everyone = false;
                List<String> emailList = notification.To.Split(';').ToList();
                foreach (string email in emailList)
                {
                    SendMail(string.Empty, notification.Message, email);
                }
            }

            notification.Send = true;
        }

        private void SendMail(string userName, string message, string email)
        {
            var bodyData = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "UserName", userName },
                    { "Title", "Aviso Importante" },
                    { "message", message }
                };

            _sendEmail.SendEmail(bodyData,
                                 "AssistanceTemplate",
                                 "Notificación de Aviso" + userName,
                                 new System.Collections.Generic.List<string>() { email }
                                );
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notification.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //////[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NotificationId,Everyone,Send,To,Message")] Notification notification)
        {
            if (id != notification.NotificationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);

                    SendNotification(notification);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.NotificationId))
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
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notification
                .FirstOrDefaultAsync(m => m.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        //////[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            _context.Notification.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notification.Any(e => e.NotificationId == id);
        }
    }
}
