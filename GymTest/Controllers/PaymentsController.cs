using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;
using GymTest.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using PagedList;
using Microsoft.Extensions.Options;

namespace GymTest.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly GymTestContext _context;

        private readonly ISendEmail _sendEmail;

        private readonly IPaymentLogic _paymentLogic;

        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        public PaymentsController(GymTestContext context, ISendEmail sendEmail, IPaymentLogic payLogic, IOptionsSnapshot<AppSettings> app)
        {
            _context = context;
            _appSettings = app;
            _sendEmail = sendEmail;
            _paymentLogic = payLogic;

        }



        // GET: Payments
        public async Task<IActionResult> Index(int? page, string sortOrder, string searchString, DateTime FromDate, DateTime ToDate)
        {
            int pageSize = int.Parse(_appSettings.Value.PageSize);
            int pageIndex = page.HasValue ? (int)page : 1;

            ViewData["PaymentSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("payment_desc") ? "payment_asc" : "payment_desc";
            ViewData["MovTypeSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("movType_desc") ? "movType_asc" : "movType_desc";
            ViewData["QuanMovTypeSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("quantType_desc") ? "quantType_asc" : "quantType_desc";
            ViewData["AmountSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("amount_desc") ? "amount_asc" : "amount_desc";
            ViewData["PayMediaSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("payMedia_desc") ? "payMedia_asc" : "payMedia_desc";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("name_desc") ? "name_asc" : "name_desc";

            var payments = from u
                           in _context.Payment.Include(p => p.MovmentType)
                                              .Include(p => p.User)
                                              .Include(p => p.PaymentMedia)
                           select u;

            if (FromDate != DateTime.MinValue)
                payments = payments.Where(s => s.PaymentDate >= FromDate);

            if (ToDate != DateTime.MinValue)
                payments = payments.Where(s => s.PaymentDate <= ToDate);


            if (!String.IsNullOrEmpty(searchString))
            {
                payments = payments.Where(s => s.User.FullName.ToLower().Contains(searchString.ToLower()) ||
                                            s.User.DocumentNumber.ToLower().Contains(searchString.ToLower()));

            }

            switch (sortOrder)
            {
                case "name_asc":
                    payments = payments.OrderBy(s => s.User.FullName);
                    break;
                case "name_desc":
                    payments = payments.OrderByDescending(s => s.User.FullName);
                    break;
                case "payment_asc":
                    payments = payments.OrderBy(s => s.PaymentDate);
                    break;
                case "payment_desc":
                    payments = payments.OrderByDescending(s => s.PaymentDate);
                    break;
                case "movType_asc":
                    payments = payments.OrderBy(s => s.MovementTypeId);
                    break;
                case "movType_desc":
                    payments = payments.OrderByDescending(s => s.MovementTypeId);
                    break;
                case "quantType_asc":
                    payments = payments.OrderBy(s => s.QuantityMovmentType);
                    break;
                case "quantType_desc":
                    payments = payments.OrderByDescending(s => s.QuantityMovmentType);
                    break;
                case "amount_asc":
                    payments = payments.OrderBy(s => s.Amount);
                    break;
                case "amount_desc":
                    payments = payments.OrderByDescending(s => s.Amount);
                    break;
                case "payMedia_asc":
                    payments = payments.OrderBy(s => s.PaymentMedia);
                    break;
                case "payMedia_desc":
                    payments = payments.OrderByDescending(s => s.PaymentMedia);
                    break;
                default:
                    payments = payments.OrderBy(s => s.PaymentDate);
                    break;
            }


            IPagedList<Payment> paymentPaged = payments.ToPagedList(pageIndex, pageSize);

            return View(paymentPaged);
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
                ViewData["UserId"] = new SelectList(_context.User.Where(u => u.Available == true), "UserId", "FullName");
            }
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
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
        //[ValidateAntiForgeryToken]
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
                        var cashMovsPayment = _context.CashMovement.Where(c => c.PaymentId == payment.PaymentId);
                        if (cashMovsPayment.Count() > 0)
                        {
                            CashMovement cashMov = cashMovsPayment.FirstOrDefault();
                            cashMov.Amount = payment.Amount;
                            cashMov.CashMovementDate = payment.PaymentDate;
                            cashMov.PaymentMediaId = payment.PaymentMediaId;

                            _context.Update(cashMov);
                        }
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
        //[ValidateAntiForgeryToken]
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
