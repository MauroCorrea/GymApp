using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using GymTest.Models;
using GymTest.Data;
using Microsoft.AspNetCore.Authorization;
using PagedList;

namespace GymTest.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly GymTestContext _context;

        public UsersController(GymTestContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(int? page, string sortOrder, string searchString)
        {
            int pageSize = 20;
            int pageIndex = page.HasValue ? (int)page : 1;

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("name_desc") ? "name_asc" : "name_desc";
            ViewData["DocSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("doc_desc") ? "doc_asc" : "doc_desc";
            ViewData["EmailSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("email_desc") ? "email_asc" : "email_desc";
            ViewData["AddressSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("address_desc") ? "address_asc" : "address_desc";
            ViewData["PhoneSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("phone_desc") ? "phone_asc" : "phone_desc";

            var users = from m in _context.User
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.FullName.ToLower().Contains(searchString.ToLower()) ||
                                    s.DocumentNumber.ToLower().Contains(searchString.ToLower()));

            }

            switch (sortOrder)
            {
                case "name_asc":
                    users = users.OrderBy(s => s.FullName);
                    break;
                case "name_desc":
                    users = users.OrderByDescending(s => s.FullName);
                    break;
                case "doc_asc":
                    users = users.OrderBy(s => s.DocumentNumber);
                    break;
                case "doc_desc":
                    users = users.OrderByDescending(s => s.DocumentNumber);
                    break;
                case "email_asc":
                    users = users.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(s => s.Email);
                    break;
                case "address_asc":
                    users = users.OrderBy(s => s.Address);
                    break;
                case "address_desc":
                    users = users.OrderByDescending(s => s.Address);
                    break;
                case "phone_asc":
                    users = users.OrderBy(s => s.Phones);
                    break;
                case "phone_desc":
                    users = users.OrderByDescending(s => s.Phones);
                    break;
                default:
                    users = users.OrderBy(s => s.FullName);
                    break;
            }


            IPagedList<User> userPaged = users.ToPagedList(pageIndex, pageSize);

            return View(userPaged);
            //return View(await users.AsNoTracking().ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(c => c.MedicalEmergency)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            ViewData["PaymentInfo"] = getPaymentInfo(user);

            return View(user);
        }

        private string getPaymentInfo(User user)
        {
            string paymentInfo = string.Empty;

            var payments = from m in _context.Payment
                           select m;
            payments = payments.Where(p => p.UserId == user.UserId);

            if (payments.Count() > 0)
            {
                var newestPayment = payments.OrderByDescending(p => p.PaymentDate).First();
                if (newestPayment.MovementTypeId > 0)
                {
                    if (newestPayment.LimitUsableDate.Date < DateTime.Now.Date)
                        return "Fecha límite de uso sobrepasada. La misma es " + newestPayment.LimitUsableDate.Date.ToShortDateString() + ".";

                    switch (newestPayment.MovementTypeId)
                    {
                        #region Mensual
                        case (int)PaymentTypeEnum.Monthly:
                            var monthsPayed = newestPayment.QuantityMovmentType;

                            var monthsUsed = DateTime.Now.Month - newestPayment.PaymentDate.Month;

                            if (DateTime.Now.Year > newestPayment.PaymentDate.Year)
                                monthsUsed += 12;

                            if (monthsUsed > monthsPayed)
                                return "Pago mensual vencido. Su último fue por " + monthsPayed + " mes(es) y se utilizaron " + monthsUsed + " mes(es).";

                            paymentInfo = "Su último pago fue por " + monthsPayed + " mes(es) y se utilizaron " + monthsUsed + " mes(es)."; ;
                            break;
                        #endregion
                        #region Por asistencias
                        case (int)PaymentTypeEnum.ByAssistances:
                            var ass = from a in _context.Assistance select a;

                            ass = ass.Where(a => a.UserId == user.UserId &&
                                            a.AssistanceDate.Date >= newestPayment.PaymentDate.Date);

                            if (ass.Count() >= newestPayment.QuantityMovmentType)
                                return "Pago por asistencias consumido. Se habilitaron " + newestPayment.QuantityMovmentType + " asistencia(s) y se utilizaron " + ass.Count() + " asistencia(s).";

                            paymentInfo = "Cantidad de asistencias restantes: " + (newestPayment.QuantityMovmentType - ass.Count()) + ".";
                            break;
                        #endregion

                        default:
                            return "Formato de pago no procesable.";
                    }

                    paymentInfo += "Fecha límite de uso del pago: " + newestPayment.LimitUsableDate.ToShortDateString() + ".";
                }
                else//error: ultimo pago sin tipo de membresía
                    return "Último pago sin tipo de membresía.";
            }
            else//error: usuario sin pagos
                return "Usuario sin pagos.";

            return paymentInfo;
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["MedicalEmergencyId"] = new SelectList(_context.Set<MedicalEmergency>(), "MedicalEmergencyId", "MedicalEmergencyDescription");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]User user) //[Bind("UserId,Token,FullName,BirthDate,DocumentNumber,Email,Address,Phones,SignInDate,Commentaries")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicalEmergencyId"] = new SelectList(_context.Set<MedicalEmergency>(), "MedicalEmergencyId", "MedicalEmergencyDescription", user.MedicalEmergencyId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["MedicalEmergencyId"] = new SelectList(_context.Set<MedicalEmergency>(), "MedicalEmergencyId", "MedicalEmergencyDescription", user.MedicalEmergencyId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm]User user) //[Bind("UserId,Token,FullName,BirthDate,DocumentNumber,Email,Address,Phones,SignInDate,Commentaries")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            ViewData["MedicalEmergencyId"] = new SelectList(_context.Set<MedicalEmergency>(), "MedicalEmergencyId", "MedicalEmergencyDescription", user.MedicalEmergencyId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(c => c.MedicalEmergency)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
