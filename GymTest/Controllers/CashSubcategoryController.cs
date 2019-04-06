using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymTest.Controllers
{
    [Authorize]
    public class CashSubcategoryController : Controller
    {
        private readonly GymTestContext _context;

        public CashSubcategoryController(GymTestContext context)
        {
            _context = context;
        }

        // GET: CashSubcategory
        public async Task<IActionResult> Index()
        {
            var gymTestContext = _context.CashSubcategory.Include(c => c.CashCategory);
            return View(await gymTestContext.ToListAsync());
        }

        // GET: CashSubcategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashSubcategory = await _context.CashSubcategory
                .Include(c => c.CashCategory)
                .FirstOrDefaultAsync(m => m.CashSubcategoryId == id);
            if (cashSubcategory == null)
            {
                return NotFound();
            }

            return View(cashSubcategory);
        }

        // GET: CashSubcategory/Create
        public IActionResult Create()
        {
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription");
            return View();
        }

        // POST: CashSubcategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CashSubcategoryId,CashSubcategoryDescription,CashCategoryId")] CashSubcategory cashSubcategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cashSubcategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashSubcategory.CashCategoryId);
            return View(cashSubcategory);
        }

        // GET: CashSubcategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashSubcategory = await _context.CashSubcategory.FindAsync(id);
            if (cashSubcategory == null)
            {
                return NotFound();
            }
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashSubcategory.CashCategoryId);
            return View(cashSubcategory);
        }

        // POST: CashSubcategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CashSubcategoryId,CashSubcategoryDescription,CashCategoryId")] CashSubcategory cashSubcategory)
        {
            if (id != cashSubcategory.CashSubcategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cashSubcategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashSubcategoryExists(cashSubcategory.CashSubcategoryId))
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
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashSubcategory.CashCategoryId);
            return View(cashSubcategory);
        }

        // GET: CashSubcategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashSubcategory = await _context.CashSubcategory
                .Include(c => c.CashCategory)
                .FirstOrDefaultAsync(m => m.CashSubcategoryId == id);
            if (cashSubcategory == null)
            {
                return NotFound();
            }

            return View(cashSubcategory);
        }

        // POST: CashSubcategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cashSubcategory = await _context.CashSubcategory.FindAsync(id);
            _context.CashSubcategory.Remove(cashSubcategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CashSubcategoryExists(int id)
        {
            return _context.CashSubcategory.Any(e => e.CashSubcategoryId == id);
        }
    }
}
