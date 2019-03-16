using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Models;
using GymTest.Data;
using GymTest.Services;

namespace GymTest.Controllers
{
    public class AssistancesController : Controller
    {
        private readonly GymTestContext _context;

        private readonly IAssistanceLogic _assistanceLogic;

        public AssistancesController(GymTestContext context, IAssistanceLogic assistanceLogic)
        {
            _context = context;
            _assistanceLogic = assistanceLogic;
        }

        // GET: Assistances
        public async Task<IActionResult> Index(string searchString, int? id)
        {
            var ret = from ass in _context.Assistance.Include("User")
                      select ass;

            if (id > 0)
            {
                ret = ret.Where(x => x.UserId == id);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                ret = ret.Where(s => s.User.FullName.ToLower().Contains(searchString.ToLower()));

            }

            return View(await ret.ToListAsync());
        }


        // GET: Assistances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistance = await _context.Assistance
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AssistanceId == id);
            if (assistance == null)
            {
                return NotFound();
            }

            return View(assistance);
        }

        // GET: Assistances/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "DocumentNumber");
            return View();
        }

        // POST: Assistances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssistanceId,AssistanceDate,UserId")] Assistance assistance)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(assistance);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                var users = from u in _context.User select u;
                var currentUser = users.Where(u => u.UserId.Equals(assistance.UserId)).FirstOrDefault();

                var assistanceInfo = _assistanceLogic.ProcessAssistance(currentUser.Token, assistance.AssistanceDate);

                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "DocumentNumber", assistance.UserId);
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
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "DocumentNumber", assistance.UserId);
            return View(assistance);
        }

        // POST: Assistances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssistanceId,AssistanceDate,UserId")] Assistance assistance)
        {
            if (id != assistance.AssistanceId)
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
                    if (!AssistanceExists(assistance.AssistanceId))
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
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "DocumentNumber", assistance.UserId);
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
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AssistanceId == id);
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
            return _context.Assistance.Any(e => e.AssistanceId == id);
        }
    }
}
