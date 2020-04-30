using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymTest.Models;
using GymTest.Data;
using Microsoft.AspNetCore.Authorization;

namespace GymTest.Controllers
{
    [Authorize]
    public class CashMovementTypeController : Controller
    {
        private readonly GymTestContext _context;

        public CashMovementTypeController(GymTestContext context)
        {
            _context = context;
        }

        // GET: CashMovementType
        public async Task<IActionResult> Index()
        {
            return View(await _context.CashMovementType.ToListAsync());
        }

        // GET: CashMovementType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashMovementType = await _context.CashMovementType
                .FirstOrDefaultAsync(m => m.CashMovementTypeId == id);
            if (cashMovementType == null)
            {
                return NotFound();
            }

            return View(cashMovementType);
        }

        // GET: CashMovementType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CashMovementType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CashMovementTypeId,CashMovementTypeDescription")] CashMovementType cashMovementType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cashMovementType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cashMovementType);
        }

        // GET: CashMovementType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashMovementType = await _context.CashMovementType.FindAsync(id);
            if (cashMovementType == null)
            {
                return NotFound();
            }
            return View(cashMovementType);
        }

        // POST: CashMovementType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CashMovementTypeId,CashMovementTypeDescription")] CashMovementType cashMovementType)
        {
            if (id != cashMovementType.CashMovementTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cashMovementType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashMovementTypeExists(cashMovementType.CashMovementTypeId))
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
            return View(cashMovementType);
        }

        // GET: CashMovementType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashMovementType = await _context.CashMovementType
                .FirstOrDefaultAsync(m => m.CashMovementTypeId == id);
            if (cashMovementType == null)
            {
                return NotFound();
            }

            return View(cashMovementType);
        }

        // POST: CashMovementType/Delete/5
        [HttpPost, ActionName("Delete")]
       //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cashMovementType = await _context.CashMovementType.FindAsync(id);
            _context.CashMovementType.Remove(cashMovementType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CashMovementTypeExists(int id)
        {
            return _context.CashMovementType.Any(e => e.CashMovementTypeId == id);
        }
    }
}
