using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Models;
using GymTest.Data;

namespace GymTest.Controllers
{
    public class CashCategoryController : Controller
    {
        private readonly GymTestContext _context;

        public CashCategoryController(GymTestContext context)
        {
            _context = context;
        }

        // GET: CashCategory
        public async Task<IActionResult> Index()
        {
            return View(await _context.CashCategory.ToListAsync());
        }

        // GET: CashCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashCategory = await _context.CashCategory
                .FirstOrDefaultAsync(m => m.CashCategoryId == id);
            if (cashCategory == null)
            {
                return NotFound();
            }

            return View(cashCategory);
        }

        // GET: CashCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CashCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CashCategoryId,CashCategoryDescription")] CashCategory cashCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cashCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cashCategory);
        }

        // GET: CashCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashCategory = await _context.CashCategory.FindAsync(id);
            if (cashCategory == null)
            {
                return NotFound();
            }
            return View(cashCategory);
        }

        // POST: CashCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CashCategoryId,CashCategoryDescription")] CashCategory cashCategory)
        {
            if (id != cashCategory.CashCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cashCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashCategoryExists(cashCategory.CashCategoryId))
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
            return View(cashCategory);
        }

        // GET: CashCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashCategory = await _context.CashCategory
                .FirstOrDefaultAsync(m => m.CashCategoryId == id);
            if (cashCategory == null)
            {
                return NotFound();
            }

            return View(cashCategory);
        }

        // POST: CashCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cashCategory = await _context.CashCategory.FindAsync(id);
            _context.CashCategory.Remove(cashCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CashCategoryExists(int id)
        {
            return _context.CashCategory.Any(e => e.CashCategoryId == id);
        }
    }
}
