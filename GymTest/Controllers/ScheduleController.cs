using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;

namespace GymTest.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly GymTestContext _context;

        public ScheduleController(GymTestContext context)
        {
            _context = context;
        }

        // GET: Schedule
        public async Task<IActionResult> Index()
        {
            var gymTestContext = _context.Schedule.Include(s => s.Field);
            return View(await gymTestContext.ToListAsync());
        }

        // GET: Schedule/Details/5
        public async Task<IActionResult> Pay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Field)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }
            else
            {
                if (schedule.Amount > 0)
                {
                    CashMovement cashMov = CreateCashMovement(schedule);
                    _context.CashMovement.Add(cashMov);
                }

                schedule.isPayed = true;
                _context.Update(schedule);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Create()
        {
            ViewBag.ReservationError = null;
            ViewData["FieldId"] = new SelectList(_context.Field, "FieldId", "FieldDescription");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleId,FieldId,ScheduleDate,StartTime,HourQuantity,Amount,ClientName,ClientPhoneNumber,isPayed")] Schedule schedule)
        {
            ViewBag.ReservationError = null;

            if (ModelState.IsValid)
            {
                var canIDoReservation = ConcurrencyControl(schedule);
                if (canIDoReservation)
                {
                    if (schedule.Amount > 0)
                    {
                        var field = _context.Field.Where(f => f.FieldId == schedule.FieldId).First();
                        schedule.Field = field;
                        CashMovement cashMov = CreateCashMovement(schedule);
                        _context.CashMovement.Add(cashMov);
                    }
                    _context.Add(schedule);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ReservationError = "La reserva no se puede concluir. Existe una reserva para esa Hora/Cancha.";
                    ViewData["FieldId"] = new SelectList(_context.Field, "FieldId", "FieldDescription", schedule.FieldId);
                    return View(schedule);
                }
            }
            ViewData["FieldId"] = new SelectList(_context.Field, "FieldId", "FieldDescription", schedule.FieldId);
            return View(schedule);
        }

        private bool ConcurrencyControl(Schedule schedule)
        {
            var allScheduleSameDayAndField = _context.Schedule.Where(s => s.FieldId == schedule.FieldId && s.ScheduleDate == s.ScheduleDate);
            if(allScheduleSameDayAndField.Count() > 0)
            {
                var mineStartHour = Convert.ToInt16(schedule.StartTime.Split(':')[0]);
                var mineStartMinute = Convert.ToInt16(schedule.StartTime.Split(':')[1]);
                var mineFinishHour = mineStartHour + schedule.HourQuantity;
                foreach(var scheduleElement in allScheduleSameDayAndField)
                {
                    var comparativeStartHour = Convert.ToInt16(scheduleElement.StartTime.Split(':')[0]);
                    var comparativeStartMinute = Convert.ToInt16(scheduleElement.StartTime.Split(':')[1]);
                    var comparativeFinishHour = comparativeStartHour + scheduleElement.HourQuantity;

                    if (mineFinishHour > comparativeStartHour && mineStartHour < comparativeFinishHour)
                    {
                        return false;
                    }
                    else if(mineFinishHour == comparativeStartHour && mineStartMinute > comparativeStartMinute)
                    {
                        return false;
                    }

                    if(mineStartHour == comparativeFinishHour && mineStartMinute < comparativeStartMinute)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private CashMovement CreateCashMovement(Schedule schedule)
        {
            var cashMov = new CashMovement();

            cashMov.Amount = schedule.Amount;
            cashMov.PaymentMediaId = 1;
            cashMov.CashMovementDate = DateTime.Now;
            cashMov.CashMovementDetails = "Reserva " + schedule.Field.FieldDescription + " " + schedule.HourQuantity + "h, " + schedule.ClientName;
            cashMov.CashMovementTypeId = 1;//1 es de tipo entrada
            cashMov.CashCategoryId = _context.CashCategory.Where(x => x.CashCategoryDescription == "Reserva Cancha").FirstOrDefault().CashCategoryId;
            cashMov.CashSubcategoryId = _context.CashSubcategory.Where(x => x.CashSubcategoryDescription == "Reserva Cancha").FirstOrDefault().CashSubcategoryId;
            cashMov.SupplierId = _context.Supplier.Where(x => x.SupplierDescription == "Reserva Cancha").FirstOrDefault().SupplierId;
            cashMov.PaymentId = null;

            return cashMov;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["FieldId"] = new SelectList(_context.Field, "FieldId", "FieldDescription", schedule.FieldId);
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,FieldId,ScheduleDate,StartTime,HourQuantity,Amount,ClientName,ClientPhoneNumber,isPayed")] Schedule schedule)
        {
            if (id != schedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ScheduleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            ViewData["FieldId"] = new SelectList(_context.Field, "FieldId", "FieldDescription", schedule.FieldId);
            return View(schedule);
        }

        // GET: Schedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Field)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedule.FindAsync(id);
            _context.Schedule.Remove(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.ScheduleId == id);
        }
    }
}
