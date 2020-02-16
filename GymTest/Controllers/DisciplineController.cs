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
    public class DisciplineController : Controller
    {
        private readonly GymTestContext _context;

        public DisciplineController(GymTestContext context)
        {
            _context = context;
        }

        // GET: Discipline
        public async Task<IActionResult> Index()
        {
            var gymTestContext = _context.Discipline.Include(d => d.Resource);
            return View(await gymTestContext.ToListAsync());
        }

        // GET: Discipline/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _context.Discipline
                .Include(d => d.Resource)
                .FirstOrDefaultAsync(m => m.DisciplineId == id);
            if (discipline == null)
            {
                return NotFound();
            }

            return View(discipline);
        }

        // GET: Discipline/Create
        public IActionResult Create()
        {
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName");
            return View();
        }

        // POST: Discipline/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DisciplineId,DisciplineDescription,ResourceId")] Discipline discipline)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discipline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName", discipline.ResourceId);
            return View(discipline);
        }

        // GET: Discipline/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _context.Discipline.FindAsync(id);
            if (discipline == null)
            {
                return NotFound();
            }
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName", discipline.ResourceId);
            return View(discipline);
        }

        // POST: Discipline/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DisciplineId,DisciplineDescription,ResourceId")] Discipline discipline)
        {
            if (id != discipline.DisciplineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discipline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisciplineExists(discipline.DisciplineId))
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
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName", discipline.ResourceId);
            return View(discipline);
        }

        // GET: Discipline/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _context.Discipline
                .Include(d => d.Resource)
                .FirstOrDefaultAsync(m => m.DisciplineId == id);
            if (discipline == null)
            {
                return NotFound();
            }

            return View(discipline);
        }

        // POST: Discipline/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discipline = await _context.Discipline.FindAsync(id);
            _context.Discipline.Remove(discipline);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DisciplineExists(int id)
        {
            return _context.Discipline.Any(e => e.DisciplineId == id);
        }
    }
}
