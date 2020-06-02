using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymTest.Controllers
{
    [Authorize]
    public class MedicalEmergencyController : Controller
    {
        private readonly GymTestContext _context;

        public MedicalEmergencyController(GymTestContext context)
        {
            _context = context;
        }

        // GET: MedicalEmergency
        public async Task<IActionResult> Index()
        {
            return View(await _context.MedicalEmergency.ToListAsync());
        }

        // GET: MedicalEmergency/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MedicalEmergency/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicalEmergencyId,MedicalEmergencyDescription")] MedicalEmergency medicalEmergency)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalEmergency);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicalEmergency);
        }

        // GET: MedicalEmergency/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalEmergency = await _context.MedicalEmergency.FindAsync(id);
            if (medicalEmergency == null)
            {
                return NotFound();
            }
            return View(medicalEmergency);
        }

        // POST: MedicalEmergency/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicalEmergencyId,MedicalEmergencyDescription")] MedicalEmergency medicalEmergency)
        {
            if (id != medicalEmergency.MedicalEmergencyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalEmergency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalEmergencyExists(medicalEmergency.MedicalEmergencyId))
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
            return View(medicalEmergency);
        }

        // GET: MedicalEmergency/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalEmergency = await _context.MedicalEmergency
                .FirstOrDefaultAsync(m => m.MedicalEmergencyId == id);
            if (medicalEmergency == null)
            {
                return NotFound();
            }

            return View(medicalEmergency);
        }

        // POST: MedicalEmergency/Delete/5
        [HttpPost, ActionName("Delete")]
       //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalEmergency = await _context.MedicalEmergency.FindAsync(id);
            _context.MedicalEmergency.Remove(medicalEmergency);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalEmergencyExists(int id)
        {
            return _context.MedicalEmergency.Any(e => e.MedicalEmergencyId == id);
        }
    }
}
