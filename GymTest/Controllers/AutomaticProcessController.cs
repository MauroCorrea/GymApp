using System;
using System.Collections.Generic;
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
    public class AutomaticProcessController : Controller
    {
        private readonly GymTestContext _context;

        public AutomaticProcessController(GymTestContext context)
        {
            _context = context;
        }

        // GET: AutomaticProcess
        public async Task<IActionResult> Index()
        {
            return View(await _context.AutomaticProcess.ToListAsync());
        }

        // GET: AutomaticProcess/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var automaticProcess = await _context.AutomaticProcess
                .FirstOrDefaultAsync(m => m.AutomaticProcessId == id);
            if (automaticProcess == null)
            {
                return NotFound();
            }

            return View(automaticProcess);
        }

        // GET: AutomaticProcess/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AutomaticProcess/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AutomaticProcessId,AutomaticProcessDesctipion,NextProcessDate")] AutomaticProcess automaticProcess)
        {
            if (ModelState.IsValid)
            {
                _context.Add(automaticProcess);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(automaticProcess);
        }

        // GET: AutomaticProcess/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var automaticProcess = await _context.AutomaticProcess.FindAsync(id);
            if (automaticProcess == null)
            {
                return NotFound();
            }
            return View(automaticProcess);
        }

        // POST: AutomaticProcess/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AutomaticProcessId,AutomaticProcessDesctipion,NextProcessDate")] AutomaticProcess automaticProcess)
        {
            if (id != automaticProcess.AutomaticProcessId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(automaticProcess);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AutomaticProcessExists(automaticProcess.AutomaticProcessId))
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
            return View(automaticProcess);
        }

        // GET: AutomaticProcess/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var automaticProcess = await _context.AutomaticProcess
                .FirstOrDefaultAsync(m => m.AutomaticProcessId == id);
            if (automaticProcess == null)
            {
                return NotFound();
            }

            return View(automaticProcess);
        }

        // POST: AutomaticProcess/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var automaticProcess = await _context.AutomaticProcess.FindAsync(id);
            _context.AutomaticProcess.Remove(automaticProcess);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AutomaticProcessExists(int id)
        {
            return _context.AutomaticProcess.Any(e => e.AutomaticProcessId == id);
        }
    }
}
