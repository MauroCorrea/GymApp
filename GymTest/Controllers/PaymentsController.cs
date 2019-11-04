using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;
using GymTest.Services;
using Microsoft.AspNetCore.Authorization;
using PagedList;

namespace GymTest.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly GymTestContext _context;

        private readonly ISendEmail _sendEmail;

        private readonly IPaymentLogic _paymentLogic;

        public PaymentsController(GymTestContext context, ISendEmail sendEmail, IPaymentLogic payLogic)
        {
            _context = context;
            _sendEmail = sendEmail;
            _paymentLogic = payLogic;

        }

        // GET: Payments
        public IActionResult Index(int? page)
        {
            var gymTestContext = _context.Payment.Include(p => p.MovmentType).Include(p => p.User).Include(p => p.PaymentMedia);

            int pageSize = 2;
            int pageNumber = (page ?? 1);

            return View(gymTestContext.ToList().OrderByDescending(x => x.PaymentDate).ToPagedList(pageNumber, pageSize));
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.MovmentType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create(int? id)
        {
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description");
            ViewData["PaymentMediaId"] = new SelectList(_context.Set<PaymentMedia>(), "PaymentMediaId", "PaymentMediaDescription");

            if (id != null)
            {
                var users = from u in _context.User select u;
                users = users.Where(u => u.UserId.Equals(id));

                ViewData["UserId"] = new SelectList(users, "UserId", "FullName");
            }
            else
            {
                ViewData["UserId"] = new SelectList(_context.User, "UserId", "FullName");
            }
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("PaymentId,PaymentDate,MovementTypeId,QuantityMovmentType,Amount,PaymentMediaId,UserId,LimitUsableDate")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                if (_paymentLogic.ProcessPayment(payment,
                        _context.User.Where(u => u.UserId == payment.UserId).First().FullName,
                        _context.User.Where(u => u.UserId == payment.UserId).First().Email))
                {
                    CashMovement cashMov = new CashMovement
                    {
                        Amount = payment.Amount,
                        PaymentMediaId = payment.PaymentMediaId,
                        CashMovementDate = payment.PaymentDate,
                        CashMovementDetails = "Movimiento de Pago",
                        CashMovementTypeId = 1,//1 es de tipo entrada
                        CashCategoryId = _context.CashCategory.Where(x => x.CashCategoryDescription == "Movimiento de pago").FirstOrDefault().CashCategoryId,
                        CashSubcategoryId = _context.CashSubcategory.Where(x => x.CashSubcategoryDescription == "Movimiento de pago").FirstOrDefault().CashSubcategoryId,
                        SupplierId = _context.Supplier.Where(x => x.SupplierDescription == "Movimiento de pago").FirstOrDefault().SupplierId,
                        PaymentId = payment.PaymentId
                    };
                    _context.Add(cashMov);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description", payment.MovementTypeId);
            ViewData["PaymentMediaId"] = new SelectList(_context.PaymentMedia, "PaymentMediaId", "PaymentMediaDescription", payment.PaymentMediaId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "FullName", payment.UserId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description", payment.MovementTypeId);
            ViewData["PaymentMediaId"] = new SelectList(_context.Set<PaymentMedia>(), "PaymentMediaId", "PaymentMediaDescription", payment.PaymentMediaId);
            ViewData["UserId"] = new SelectList(_context.User.Where(u => u.UserId == payment.UserId), "UserId", "FullName", payment.UserId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("PaymentId,PaymentDate,MovementTypeId,QuantityMovmentType,Amount,PaymentMediaId,UserId,LimitUsableDate")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_paymentLogic.ProcessPayment(payment,
                        _context.User.Where(u => u.UserId == payment.UserId).First().FullName,
                        _context.User.Where(u => u.UserId == payment.UserId).First().Email))
                    {
                        CashMovement cashMov = _context.CashMovement.Where(c => c.PaymentId == payment.PaymentId).FirstOrDefault();
                        cashMov.Amount = payment.Amount;
                        cashMov.CashMovementDate = payment.PaymentDate;
                        cashMov.PaymentMediaId = payment.PaymentMediaId;

                        _context.Update(cashMov);
                        _context.SaveChanges();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description", payment.MovementTypeId);
            ViewData["PaymentMediaId"] = new SelectList(_context.Set<PaymentMedia>(), "PaymentMediaId", "PaymentMediaDescription", payment.PaymentMediaId);
            ViewData["UserId"] = new SelectList(_context.User.Where(u => u.UserId == payment.UserId), "UserId", "FullName", payment.UserId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.MovmentType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            CashMovement cashMovement = _context.CashMovement.Where(c => c.PaymentId == id).FirstOrDefault();

            _context.Payment.Remove(payment);
            _context.CashMovement.Remove(cashMovement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.PaymentId == id);
        }
    }
}
