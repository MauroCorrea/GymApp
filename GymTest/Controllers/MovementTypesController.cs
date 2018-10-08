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
    public class MovementTypesController : Controller
    {
        private readonly GymTestContext _context;

        public MovementTypesController(GymTestContext context)
        {
            _context = context;
        }

        // GET: MovementTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.MovementType.ToListAsync());
        }

        // GET: MovementTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movementType = await _context.MovementType
                .FirstOrDefaultAsync(m => m.MovementTypeId == id);
            if (movementType == null)
            {
                return NotFound();
            }

            return View(movementType);
        }

        // GET: MovementTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MovementTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovementTypeId,Description")] MovementType movementType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movementType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movementType);
        }

        // GET: MovementTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movementType = await _context.MovementType.FindAsync(id);
            if (movementType == null)
            {
                return NotFound();
            }
            return View(movementType);
        }

        // POST: MovementTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovementTypeId,Description")] MovementType movementType)
        {
            if (id != movementType.MovementTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movementType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovementTypeExists(movementType.MovementTypeId))
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
            return View(movementType);
        }

        // GET: MovementTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movementType = await _context.MovementType
                .FirstOrDefaultAsync(m => m.MovementTypeId == id);
            if (movementType == null)
            {
                return NotFound();
            }

            return View(movementType);
        }

        // POST: MovementTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movementType = await _context.MovementType.FindAsync(id);
            _context.MovementType.Remove(movementType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovementTypeExists(int id)
        {
            return _context.MovementType.Any(e => e.MovementTypeId == id);
        }
    }
}
