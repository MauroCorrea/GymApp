using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Models;

namespace GymTest.Controllers
{
    public class AssistancesController : Controller
    {
        private readonly GymTestContext _context;

        public AssistancesController(GymTestContext context)
        {
            _context = context;
        }

        // GET: Assistances
        public async Task<IActionResult> Index()
        {
            return View(await _context.Assistance.ToListAsync());
        }

        // GET: Assistances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistance = await _context.Assistance
                .FirstOrDefaultAsync(m => m.ID == id);
            if (assistance == null)
            {
                return NotFound();
            }

            return View(assistance);
        }

        // GET: Assistances/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Assistances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,AssistanceDate")] Assistance assistance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assistance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(assistance);
        }

        // GET: Assistances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistance = await _context.Assistance.FindAsync(id);
            if (assistance == null)
            {
                return NotFound();
            }
            return View(assistance);
        }

        // POST: Assistances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,AssistanceDate")] Assistance assistance)
        {
            if (id != assistance.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assistance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssistanceExists(assistance.ID))
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
            return View(assistance);
        }

        // GET: Assistances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistance = await _context.Assistance
                .FirstOrDefaultAsync(m => m.ID == id);
            if (assistance == null)
            {
                return NotFound();
            }

            return View(assistance);
        }

        // POST: Assistances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assistance = await _context.Assistance.FindAsync(id);
            _context.Assistance.Remove(assistance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssistanceExists(int id)
        {
            return _context.Assistance.Any(e => e.ID == id);
        }
    }
}
