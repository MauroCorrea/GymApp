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
    public class PaymentMediaController : Controller
    {
        private readonly GymTestContext _context;

        public PaymentMediaController(GymTestContext context)
        {
            _context = context;
        }

        // GET: PaymentMedia
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaymentMedia.ToListAsync());
        }

        // GET: PaymentMedia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentMedia = await _context.PaymentMedia
                .FirstOrDefaultAsync(m => m.PaymentMediaId == id);
            if (paymentMedia == null)
            {
                return NotFound();
            }

            return View(paymentMedia);
        }

        // GET: PaymentMedia/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaymentMedia/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //////[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentMediaId,PaymentMediaDescription")] PaymentMedia paymentMedia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentMedia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paymentMedia);
        }

        // GET: PaymentMedia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentMedia = await _context.PaymentMedia.FindAsync(id);
            if (paymentMedia == null)
            {
                return NotFound();
            }
            return View(paymentMedia);
        }

        // POST: PaymentMedia/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //////[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentMediaId,PaymentMediaDescription")] PaymentMedia paymentMedia)
        {
            if (id != paymentMedia.PaymentMediaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentMedia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentMediaExists(paymentMedia.PaymentMediaId))
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
            return View(paymentMedia);
        }

        // GET: PaymentMedia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentMedia = await _context.PaymentMedia
                .FirstOrDefaultAsync(m => m.PaymentMediaId == id);
            if (paymentMedia == null)
            {
                return NotFound();
            }

            return View(paymentMedia);
        }

        // POST: PaymentMedia/Delete/5
        [HttpPost, ActionName("Delete")]
        //////[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentMedia = await _context.PaymentMedia.FindAsync(id);
            _context.PaymentMedia.Remove(paymentMedia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentMediaExists(int id)
        {
            return _context.PaymentMedia.Any(e => e.PaymentMediaId == id);
        }
    }
}
