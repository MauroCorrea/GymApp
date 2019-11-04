using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;
using GymTest.Models.ReportModels;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using GymTest.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;

namespace GymTest.Controllers
{
    public class UserReportController : Controller
    {
        private readonly GymTestContext _context;
        private readonly ISendEmail _sendEmail;
        private IHostingEnvironment _env;
        private readonly IOptionsSnapshot<AppSettings> _appSettings;
        private UserManager<IdentityUser> _userManager;

        public UserReportController(GymTestContext context, ISendEmail sendEmail, IHostingEnvironment env, IOptionsSnapshot<AppSettings> app, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            _sendEmail = sendEmail;
            _env = env;
            _appSettings = app;
        }

        // GET: UserReport
        public async Task<IActionResult> Index()
        {
            var gymTestContext = _context.UserReport.Include(u => u.MovmentType).Include(u => u.PaymentMedia);
            return View(await gymTestContext.ToListAsync());
        }

        // GET: UserReport/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userReport = await _context.UserReport
                .Include(u => u.MovmentType)
                .Include(u => u.PaymentMedia)
                .FirstOrDefaultAsync(m => m.UserReportId == id);
            if (userReport == null)
            {
                return NotFound();
            }


            var singInFrom = userReport.SignInDateFrom == null ? DateTime.MinValue : userReport.SignInDateFrom;
            var singInTo = userReport.SignInDateTo == null ? DateTime.MaxValue : userReport.SignInDateTo;
            var countAssistFrom = userReport.AssitanceCountFrom == null ? 0 : userReport.AssitanceCountFrom;
            var countAssistTo = userReport.AssitanceCountTo == null ? 9999 : userReport.AssitanceCountTo;
            var assistFrom = userReport.AssitanceFrom == null ? DateTime.MinValue : userReport.AssitanceFrom;
            var assistTo = userReport.AssitanceTo == null ? DateTime.MaxValue : userReport.AssitanceTo;
            var ageFrom = userReport.AgeFrom == null ? DateTime.Now : DateTime.Now.AddYears((int)userReport.AgeFrom * -1);
            var ageTo = userReport.AgeTo == null ? DateTime.MinValue : DateTime.Now.AddYears((int)userReport.AgeTo * -1);

            var payFrom = userReport.PayDateFrom == null ? DateTime.MinValue : userReport.PayDateFrom;
            var payTo = userReport.PayDateTo == null ? DateTime.MaxValue : userReport.PayDateTo;
            var moveTypeId = userReport.MovementTypeId;
            var paymentMediaId = userReport.PaymentMediaId;


            List<UserReportModel> result = new List<UserReportModel>();

            var users = _context.User.Where(x => x.SignInDate >= singInFrom && x.SignInDate <= singInTo
                                            && x.BirthDate <= ageFrom && x.BirthDate >= ageTo);

            foreach(User user in users)
            {
                var assistances = _context.Assistance.Where(x => x.UserId == user.UserId && x.AssistanceDate >= assistFrom
                                                            && x.AssistanceDate <= assistTo).OrderBy(o => o.AssistanceDate);

                List<Payment> payments = new List<Payment>();
                if (assistances.Count() >= countAssistFrom && assistances.Count() <= countAssistTo)
                {
                    var auxPayments = _context.Payment.Where(x => x.UserId == user.UserId && x.PaymentDate >= payFrom && x.PaymentDate <= payTo);
                    if(moveTypeId != null)
                    {
                        auxPayments = auxPayments.Where(x => x.MovementTypeId == (int)moveTypeId);
                    }
                    if(paymentMediaId != null)
                    {
                        auxPayments = auxPayments.Where(x => x.PaymentMediaId == (int)paymentMediaId);
                    }

                    payments = auxPayments.OrderBy(o => o.PaymentDate).ToList();
                }

                result.Add(new UserReportModel { Name = user.FullName,
                    BirthDate = user.BirthDate,
                    SingInDate = user.SignInDate,
                    AssistanceCount = assistances.Count(),
                    AssistFrom = assistances.Count() > 0 ? assistances.First().AssistanceDate : DateTime.MinValue,
                    AssistTo = assistances.Count() > 0 ? assistances.Last().AssistanceDate : DateTime.MaxValue,
                    PaymentCount = payments.Count(),
                    PaymentFrom = payments.Count() > 0 ? payments.First().PaymentDate : DateTime.MinValue,
                    PaymentTo = payments.Count() > 0 ? payments.Last().PaymentDate : DateTime.MaxValue,
                    paymentType = moveTypeId != null ? _context.MovementType.Where(x => x.MovementTypeId == (int)moveTypeId).FirstOrDefault().Description : "",
                    paymentMedia = paymentMediaId != null ? _context.PaymentMedia.Where(x => x.PaymentMediaId == (int)paymentMediaId).FirstOrDefault().PaymentMediaDescription : ""
                });
            }

            return await ExportToExcel(result);

            //return View(userReport);
        }

        private async Task<IActionResult> ExportToExcel(List<UserReportModel> result)
        {
            string path = _env.WebRootPath;
            string Ruta_Publica_Excel = path + "/ReporteUsuario_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";

            ExcelPackage Package = new ExcelPackage(new System.IO.FileInfo(Ruta_Publica_Excel));
            var Hoja_1 = Package.Workbook.Worksheets.Add("Contenido_1");

            /*------------------------------------------------------*/
            int rowNum = 2;
            int originalRowNum = rowNum;

            Hoja_1.Cells["B" + rowNum].Value = "Nombre";
            Hoja_1.Cells["C" + rowNum].Value = "Fecha Nacimiento";
            Hoja_1.Cells["D" + rowNum].Value = "Fecha Ingreso";
            Hoja_1.Cells["E" + rowNum].Value = "Cantidad Entradas";
            Hoja_1.Cells["F" + rowNum].Value = "Fecha 1ra Asistencia";
            Hoja_1.Cells["G" + rowNum].Value = "Fecha Ultima Assistencia";
            Hoja_1.Cells["H" + rowNum].Value = "Tipo Pago";
            Hoja_1.Cells["I" + rowNum].Value = "Medio de Pago";
            Hoja_1.Cells["J" + rowNum].Value = "Cantidad Pagos";
            Hoja_1.Cells["K" + rowNum].Value = "Fecha 1er Pago";
            Hoja_1.Cells["L" + rowNum].Value = "Fecha Ultimo Pago";

            Hoja_1.Cells["B" + rowNum + ":N" + rowNum].Style.Font.Bold = true;
            Hoja_1.Cells["B" + rowNum + ":N" + rowNum].Style.Font.Size = 15;

            foreach (UserReportModel row in result)
            {
                rowNum++;
                Hoja_1.Cells["B" + rowNum].Value = row.Name;
                Hoja_1.Cells["C" + rowNum].Value = row.BirthDate.ToString("dd/MM/yyyy");
                Hoja_1.Cells["D" + rowNum].Value = ((DateTime)row.SingInDate).ToString("dd/MM/yyyy");
                Hoja_1.Cells["E" + rowNum].Value = row.AssistanceCount;
                Hoja_1.Cells["F" + rowNum].Value = row.AssistFrom.ToString("dd/MM/yyyy");
                Hoja_1.Cells["G" + rowNum].Value = row.AssistTo.ToString("dd/MM/yyyy");
                Hoja_1.Cells["H" + rowNum].Value = row.paymentType;
                Hoja_1.Cells["I" + rowNum].Value = row.paymentMedia;
                Hoja_1.Cells["J" + rowNum].Value = row.PaymentCount;
                Hoja_1.Cells["K" + rowNum].Value = row.PaymentFrom.ToString("dd/MM/yyyy");
                Hoja_1.Cells["L" + rowNum].Value = row.PaymentTo.ToString("dd/MM/yyyy");
            }

            /*------------------------------------------------------*/

            Package.Save();

            //SendMail
            var user = await _userManager.GetUserAsync(User);
            var userEmail = user.Email;
            var userName = user.UserName;
            if (string.IsNullOrEmpty(userEmail))
                userEmail = _appSettings.Value.EmailConfiguration_Username;
            if (string.IsNullOrEmpty(userName))
                userName = "Administrador";

            var bodyData = new Dictionary<string, string>
                {
                    { "UserName", userName },
                    { "Title", "Envío de reporte de usuario." },
                    { "message", "Descargue el archivo adjunto." }
                };

            _sendEmail.SendEmail(bodyData,
                                 "ReportTemplate",
                                 "Reporte de movimientos de caja",
                                 new List<string>() { userEmail },
                                 new List<string>() { Ruta_Publica_Excel }
                                );

            if ((System.IO.File.Exists(Ruta_Publica_Excel)))
            {
                System.IO.File.Delete(Ruta_Publica_Excel);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: UserReport/Create
        public IActionResult Create()
        {
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description");
            ViewData["PaymentMediaId"] = new SelectList(_context.PaymentMedia, "PaymentMediaId", "PaymentMediaDescription");
            return View();
        }

        // POST: UserReport/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserReportId,SignInDateFrom,SignInDateTo,AssitanceFrom,AssitanceTo,AgeFrom,AgeTo,AssitanceCountFrom,AssitanceCountTo,PayDateFrom,PayDateTo,MovementTypeId,PaymentMediaId")] UserReport userReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description", userReport.MovementTypeId);
            ViewData["PaymentMediaId"] = new SelectList(_context.PaymentMedia, "PaymentMediaId", "PaymentMediaDescription", userReport.PaymentMediaId);
            return View(userReport);
        }

        // GET: UserReport/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userReport = await _context.UserReport.FindAsync(id);
            if (userReport == null)
            {
                return NotFound();
            }
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description", userReport.MovementTypeId);
            ViewData["PaymentMediaId"] = new SelectList(_context.PaymentMedia, "PaymentMediaId", "PaymentMediaDescription", userReport.PaymentMediaId);
            return View(userReport);
        }

        // POST: UserReport/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserReportId,SignInDateFrom,SignInDateTo,AssitanceFrom,AssitanceTo,AgeFrom,AgeTo,AssitanceCountFrom,AssitanceCountTo,PayDateFrom,PayDateTo,MovementTypeId,PaymentMediaId")] UserReport userReport)
        {
            if (id != userReport.UserReportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserReportExists(userReport.UserReportId))
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
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Description", userReport.MovementTypeId);
            ViewData["PaymentMediaId"] = new SelectList(_context.PaymentMedia, "PaymentMediaId", "PaymentMediaDescription", userReport.PaymentMediaId);
            return View(userReport);
        }

        // GET: UserReport/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userReport = await _context.UserReport
                .Include(u => u.MovmentType)
                .Include(u => u.PaymentMedia)
                .FirstOrDefaultAsync(m => m.UserReportId == id);
            if (userReport == null)
            {
                return NotFound();
            }

            return View(userReport);
        }

        // POST: UserReport/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userReport = await _context.UserReport.FindAsync(id);
            _context.UserReport.Remove(userReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserReportExists(int id)
        {
            return _context.UserReport.Any(e => e.UserReportId == id);
        }
    }
}
