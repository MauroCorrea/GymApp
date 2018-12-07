using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Models;
using GymTest.Data;

namespace GymTest.Controllers
{
    public class CashMovementController : Controller
    {
        private readonly GymTestContext _context;

        public CashMovementController(GymTestContext context)
        {
            _context = context;
        }

        // GET: CashMovement
        public async Task<IActionResult> Index()
        {
            var gymTestContext = _context.CashMovement
                                         .Include(c => c.CashCategory)
                                         .Include(c => c.CashMovementType)
                                         .Include(c => c.Supplier);
            return View(await gymTestContext.ToListAsync());
        }

        // GET: CashMovement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashMovement = await _context.CashMovement
                .Include(c => c.CashCategory)
                .Include(c => c.CashMovementType)
                .Include(c => c.Supplier)
                .FirstOrDefaultAsync(m => m.CashMovementId == id);
            if (cashMovement == null)
            {
                return NotFound();
            }

            return View(cashMovement);
        }

        // GET: CashMovement/Create
        public IActionResult Create()
        {
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription");
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription");
            ViewData["SupplierId"] = new SelectList(_context.Set<Supplier>(), "SupplierId", "SupplierDescription");
            return View();
        }

        // POST: CashMovement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CashMovementId,CashMovementDetails,Amount,CashMovementTypeId,CashCategoryId,SupplierId")] CashMovement cashMovement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cashMovement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashMovement.CashCategoryId);
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription", cashMovement.CashMovementTypeId);
            ViewData["SupplierId"] = new SelectList(_context.Set<Supplier>(), "SupplierId", "SupplierDescription", cashMovement.SupplierId);
            return View(cashMovement);
        }

        // GET: CashMovement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashMovement = await _context.CashMovement.FindAsync(id);
            if (cashMovement == null)
            {
                return NotFound();
            }
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashMovement.CashCategoryId);
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription", cashMovement.CashMovementTypeId);
            ViewData["SupplierId"] = new SelectList(_context.Set<Supplier>(), "SupplierId", "SupplierDescription", cashMovement.SupplierId);
            return View(cashMovement);
        }

        // POST: CashMovement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CashMovementId,CashMovementDetails,Amount,CashMovementTypeId,CashCategoryId,SupplierId")] CashMovement cashMovement)
        {
            if (id != cashMovement.CashMovementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cashMovement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashMovementExists(cashMovement.CashMovementId))
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
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashMovement.CashCategoryId);
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription", cashMovement.CashMovementTypeId);
            ViewData["SupplierId"] = new SelectList(_context.Set<Supplier>(), "SupplierId", "SupplierDescription", cashMovement.SupplierId);
            return View(cashMovement);
        }

        // GET: CashMovement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashMovement = await _context.CashMovement
                .Include(c => c.CashCategory)
                .Include(c => c.CashMovementType)
                .Include(c => c.Supplier)
                .FirstOrDefaultAsync(m => m.CashMovementId == id);
            if (cashMovement == null)
            {
                return NotFound();
            }

            return View(cashMovement);
        }

        // POST: CashMovement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cashMovement = await _context.CashMovement.FindAsync(id);
            _context.CashMovement.Remove(cashMovement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CashMovementExists(int id)
        {
            return _context.CashMovement.Any(e => e.CashMovementId == id);
        }
    }
}
