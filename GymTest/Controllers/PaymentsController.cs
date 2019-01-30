using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;
using GymTest.Services;

namespace GymTest.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly GymTestContext _context;

        private readonly ISendEmail _sendEmail;

        private readonly IPaymentLogic _paymentLogic;

        public PaymentsController(GymTestContext context, ISendEmail sendEmail, IPaymentLogic payLogic)
        {
            _context = context;
            _sendEmail = sendEmail;
            _paymentLogic = payLogic;

        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var gymTestContext = _context.Payment.Include(p => p.MovmentType).Include(p => p.User);
            return View(await gymTestContext.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.MovmentType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create(int? id)
        {
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description");

            if (id != null)
            {
                var users = from u in _context.User select u;
                users = users.Where(u => u.UserId.Equals(id));

                ViewData["UserId"] = new SelectList(users, "UserId", "FullName");
            }
            else
            {
                ViewData["UserId"] = new SelectList(_context.User, "UserId", "FullName");
            }
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,PaymentDate,MovementTypeId,QuantityMovmentType,Amount,UserId,LimitUsableDate")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                if (_paymentLogic.ProcessPayment(payment,
                        _context.User.Where(u => u.UserId == payment.UserId).First().FullName,
                        _context.User.Where(u => u.UserId == payment.UserId).First().Email))
                    return RedirectToAction(nameof(Index));
            }
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description", payment.MovementTypeId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "FullName", payment.UserId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description", payment.MovementTypeId);
            ViewData["UserId"] = new SelectList(_context.User.Where(u => u.UserId == payment.UserId), "UserId", "FullName", payment.UserId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,PaymentDate,MovementTypeId,QuantityMovmentType,Amount,UserId,LimitUsableDate")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_paymentLogic.ProcessPayment(payment,
                    _context.User.Where(u => u.UserId == payment.UserId).First().FullName,
                        _context.User.Where(u => u.UserId == payment.UserId).First().Email))
                        return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description", payment.MovementTypeId);
            ViewData["UserId"] = new SelectList(_context.User.Where(u => u.UserId == payment.UserId), "UserId", "FullName", payment.UserId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.MovmentType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            _context.Payment.Remove(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.PaymentId == id);
        }
    }
}
