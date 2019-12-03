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
    public class WorkdayController : Controller
    {
        private readonly GymTestContext _context;

        public WorkdayController(GymTestContext context)
        {
            _context = context;
        }

        // GET: Workday
        public async Task<IActionResult> Index()
        {
            var gymTestContext = _context.Workday.Include(w => w.Resource);
            return View(await gymTestContext.ToListAsync());
        }

        // GET: Workday/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workday = await _context.Workday
                .Include(w => w.Resource)
                .FirstOrDefaultAsync(m => m.WorkdayId == id);
            if (workday == null)
            {
                return NotFound();
            }

            return View(workday);
        }

        // GET: Workday/Create
        public IActionResult Create()
        {
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName");
            return View();
        }

        // POST: Workday/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkdayId,ResourceId,WorkingDate,QuantityOne,QuantityTwo")] Workday workday)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workday);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName", workday.ResourceId);
            return View(workday);
        }

        // GET: Workday/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workday = await _context.Workday.FindAsync(id);
            if (workday == null)
            {
                return NotFound();
            }
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName", workday.ResourceId);
            return View(workday);
        }

        // POST: Workday/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkdayId,ResourceId,WorkingDate,QuantityOne,QuantityTwo")] Workday workday)
        {
            if (id != workday.WorkdayId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workday);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkdayExists(workday.WorkdayId))
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
            ViewData["ResourceId"] = new SelectList(_context.Resource, "ResourceId", "FullName", workday.ResourceId);
            return View(workday);
        }

        // GET: Workday/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var workday = await _context.Workday
        //        .Include(w => w.Resource)
        //        .FirstOrDefaultAsync(m => m.WorkdayId == id);
        //    if (workday == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(workday);
        //}

        //// POST: Workday/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var workday = await _context.Workday.FindAsync(id);
        //    _context.Workday.Remove(workday);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool WorkdayExists(int id)
        {
            return _context.Workday.Any(e => e.WorkdayId == id);
        }
    }
}
