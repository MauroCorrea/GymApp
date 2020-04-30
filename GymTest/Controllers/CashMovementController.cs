using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Models;
using GymTest.Data;
using OfficeOpenXml;
using System;
using Microsoft.AspNetCore.Authorization;
using GymTest.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PagedList;

namespace GymTest.Controllers
{
    [Authorize]
    public class CashMovementController : Controller
    {
        private readonly GymTestContext _context;
        private readonly ISendEmail _sendEmail;
        private IHostingEnvironment _env;
        private readonly IOptionsSnapshot<AppSettings> _appSettings;
        private UserManager<IdentityUser> _userManager;

        private readonly ITimezoneLogic _timeZone;

        private readonly ILogger<IPaymentLogic> _logger;

        public CashMovementController(GymTestContext context, ISendEmail sendEmail, IHostingEnvironment env, IOptionsSnapshot<AppSettings> app, UserManager<IdentityUser> userManager, ILogger<IPaymentLogic> logger, ITimezoneLogic timeZone)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _sendEmail = sendEmail;
            _env = env;
            _appSettings = app;
            _timeZone = timeZone;
        }

        // GET: CashMovement
        public IActionResult Index(int? page, string sortOrder)
        {
            int pageSize = int.Parse(_appSettings.Value.PageSize);
            int pageIndex = page.HasValue ? (int)page : 1;

            ViewData["CashMovSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("cashMov_desc") ? "cashMov_asc" : "cashMov_desc";
            ViewData["AmountSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("amount_desc") ? "amount_asc" : "amount_desc";
            ViewData["CashMovDateSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("cashMovDate_desc") ? "cashMovDate_asc" : "cashMovDate_desc";
            ViewData["CashMovTypeSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("cashMovType_desc") ? "cashMovType_asc" : "cashMovType_desc";
            ViewData["PayMediaSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("payMedia_desc") ? "payMedia_asc" : "payMedia_desc";
            ViewData["CashCatSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("cashCat_desc") ? "cashCat_asc" : "cashCat_desc";
            ViewData["CashSubCatSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("cashSubCat_desc") ? "cashSubCat_asc" : "cashSubCat_desc";

            var cashMovements = _context.CashMovement
                                         .Include(c => c.CashCategory)
                                         .Include(c => c.PaymentMedia)
                                         .Include(c => c.CashSubcategory)
                                         .Include(c => c.CashMovementType)
                                         .Include(c => c.Supplier);


            switch (sortOrder)
            {
                case "cashMov_asc":
                    cashMovements.ToList().OrderBy(s => s.CashMovementDetails);
                    break;
                case "cashMov_desc":
                    cashMovements.ToList().OrderByDescending(s => s.CashMovementDetails);
                    break;
                case "amount_asc":
                    cashMovements.ToList().OrderBy(s => s.Amount);
                    break;
                case "amount_desc":
                    cashMovements.ToList().OrderByDescending(s => s.Amount);
                    break;
                case "cashMovDate_asc":
                    cashMovements.ToList().OrderBy(s => s.CashMovementDate);
                    break;
                case "cashMovDate_desc":
                    cashMovements.ToList().OrderByDescending(s => s.CashMovementDate);
                    break;
                case "cashMovType_asc":
                    cashMovements.ToList().OrderBy(s => s.CashMovementType);
                    break;
                case "cashMovType_desc":
                    cashMovements.ToList().OrderByDescending(s => s.CashMovementType);
                    break;
                case "payMedia_asc":
                    cashMovements.ToList().OrderBy(s => s.PaymentMedia);
                    break;
                case "payMedia_desc":
                    cashMovements.ToList().OrderByDescending(s => s.PaymentMedia);
                    break;
                case "cashCat_asc":
                    cashMovements.ToList().OrderBy(s => s.CashCategory);
                    break;
                case "cashCat_desc":
                    cashMovements.ToList().OrderByDescending(s => s.CashCategory);
                    break;
                case "cashSubCat_asc":
                    cashMovements.ToList().OrderBy(s => s.CashSubcategory);
                    break;
                case "cashSubCat_desc":
                    cashMovements.ToList().OrderByDescending(s => s.CashSubcategory);
                    break;
                default:
                    cashMovements.ToList().OrderBy(s => s.CashMovementDate);
                    break;
            }

            IPagedList<CashMovement> acashMovementPaged = cashMovements.ToPagedList(pageIndex, pageSize);
            return View(acashMovementPaged);
        }

        public async Task<IActionResult> ExportToExcel(DateTime FromDate, DateTime ToDate)
        {

            if (FromDate == DateTime.MinValue)
                FromDate = _timeZone.GetCurrentDateTime(DateTime.Now).AddDays(-7);
            if (ToDate == DateTime.MinValue)
                ToDate = _timeZone.GetCurrentDateTime(DateTime.Now).AddDays(1);

            var cashMovs = _context.CashMovement.Where(cm => cm.CashMovementDate >= FromDate && cm.CashMovementDate < ToDate);

            cashMovs.OrderByDescending(x => x.CashMovementDate);

            string path = _env.WebRootPath;
            string Ruta_Publica_Excel = path + "/MovimientosDeCaja_" + _timeZone.GetCurrentDateTime(DateTime.Now).ToString("ddMMyyyyHHmmss") + ".xlsx";

            ExcelPackage Package = new ExcelPackage(new System.IO.FileInfo(Ruta_Publica_Excel));
            var Hoja_1 = Package.Workbook.Worksheets.Add("Contenido_1");

            /*------------------------------------------------------*/
            int rowNum = 1;

            Hoja_1.Cells["B" + rowNum].Value = "Desde:";
            Hoja_1.Cells["C" + rowNum].Value = FromDate.ToShortDateString();
            Hoja_1.Cells["D" + rowNum].Value = "Hasta:";
            Hoja_1.Cells["E" + rowNum].Value = ToDate.ToShortDateString();

            /*------------------------------------------------------*/
            rowNum = 2;
            int originalRowNum = rowNum;

            Hoja_1.Cells["B" + rowNum].Value = "Detalles";
            Hoja_1.Cells["C" + rowNum].Value = "Tipo";
            Hoja_1.Cells["D" + rowNum].Value = "Categoría";
            Hoja_1.Cells["E" + rowNum].Value = "Subcategoría";
            Hoja_1.Cells["F" + rowNum].Value = "Medio de Pago";
            Hoja_1.Cells["G" + rowNum].Value = "Fecha";
            Hoja_1.Cells["H" + rowNum].Value = "Monto";
            Hoja_1.Cells["I" + rowNum].Value = "Proveedor";

            Hoja_1.Cells["K" + rowNum].Value = "Saldos por Medio de pago";

            Hoja_1.Cells["N" + rowNum].Value = "Total movimientos";

            Hoja_1.Cells["B" + rowNum + ":N" + rowNum].Style.Font.Bold = true;
            Hoja_1.Cells["B" + rowNum + ":N" + rowNum].Style.Font.Size = 15;

            Dictionary<string, float> balances = new Dictionary<string, float>();

            foreach (CashMovement row in cashMovs)
            {
                row.CashMovementType = _context.CashMovementType.Where(x => x.CashMovementTypeId == row.CashMovementTypeId).First();
                row.CashCategory = _context.CashCategory.Where(x => x.CashCategoryId == row.CashCategoryId).First();
                row.CashSubcategory = _context.CashSubcategory.Where(x => x.CashSubcategoryId == row.CashSubcategoryId).First();
                row.PaymentMedia = _context.PaymentMedia.Where(x => x.PaymentMediaId == row.PaymentMediaId).First();
                row.Supplier = _context.Supplier.Where(x => x.SupplierId == row.SupplierId).First();

                rowNum++;
                Hoja_1.Cells["B" + rowNum].Value = row.CashMovementDetails;
                Hoja_1.Cells["C" + rowNum].Value = row.CashMovementType.CashMovementTypeDescription;
                Hoja_1.Cells["D" + rowNum].Value = row.CashCategory.CashCategoryDescription;
                Hoja_1.Cells["E" + rowNum].Value = row.CashSubcategory.CashSubcategoryDescription;
                Hoja_1.Cells["F" + rowNum].Value = row.PaymentMedia.PaymentMediaDescription;
                Hoja_1.Cells["G" + rowNum].Value = row.CashMovementDate.ToString("dd/MM/yyyy");
                Hoja_1.Cells["H" + rowNum].Value = row.CashMovementTypeId == 1 ? row.Amount : (row.Amount * (-1));
                Hoja_1.Cells["I" + rowNum].Value = row.Supplier.SupplierDescription;

                if (balances.ContainsKey(row.PaymentMedia.PaymentMediaDescription))
                {
                    float totalAmount = balances[row.PaymentMedia.PaymentMediaDescription] + (float)(row.CashMovementTypeId == 1 ? row.Amount : (row.Amount * (-1)));
                    balances[row.PaymentMedia.PaymentMediaDescription] = totalAmount;
                }
                else
                {
                    balances.Add(row.PaymentMedia.PaymentMediaDescription, (float)(row.CashMovementTypeId == 1 ? row.Amount : (row.Amount * (-1))));
                }
            }

            if (cashMovs.Count() > 0)
            {
                //Total movs sum:
                Hoja_1.Cells["N" + (originalRowNum + 1)].Formula = "SUM(H" + (originalRowNum + 1) + ":H" + rowNum + ")";

                //Total by paymentType
                int rowNumTotal = 2;
                foreach (string key in balances.Keys)
                {
                    rowNumTotal++;
                    Hoja_1.Cells["k" + rowNumTotal].Value = key;
                    Hoja_1.Cells["L" + rowNumTotal].Value = balances[key];
                }
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
                    { "Title", "Envío de movimientos de caja." },
                    { "message", "Descargue el archivo adjunto." }
                };

            _sendEmail.SendEmail(bodyData,
                                 "ReportTemplate",
                                 "Reporte de movimientos de caja",
                                 new List<string>() { userEmail },
                                 new List<string>() { Ruta_Publica_Excel }
                                );
            try
            {
                if ((System.IO.File.Exists(Ruta_Publica_Excel)))
                {
                    System.IO.File.Delete(Ruta_Publica_Excel);
                }
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error Processing Payment. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error Processing Payment. Detail: " + ex.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: CashMovement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashMovement = await _context.CashMovement
                .Include(c => c.CashCategory)
                .Include(c => c.CashSubcategory)
                .Include(c => c.CashMovementType)
                .Include(c => c.Supplier)
                .Include(c => c.PaymentMedia)
                .FirstOrDefaultAsync(m => m.CashMovementId == id);
            if (cashMovement == null)
            {
                return NotFound();
            }

            return View(cashMovement);
        }

        // GET: CashMovement/Create
        public IActionResult Create()
        {
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory.Where(x => x.CashCategoryDescription != "Movimiento de pago"), "CashCategoryId", "CashCategoryDescription");
            ViewData["CashSubcategoryId"] = new SelectList(_context.CashSubcategory.Where(x => x.CashSubcategoryDescription != "Movimiento de pago"), "CashSubcategoryId", "CashSubcategoryDescription");
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription");
            ViewData["PaymentMediaId"] = new SelectList(_context.Set<PaymentMedia>(), "PaymentMediaId", "PaymentMediaDescription");
            ViewData["SupplierId"] = new SelectList(_context.Supplier.Where(x => x.SupplierDescription != "Movimiento de pago"), "SupplierId", "SupplierDescription");
            return View();
        }

        // POST: CashMovement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CashMovementId,CashMovementDate,CashMovementDetails,Amount,CashMovementTypeId,PaymentMediaId,CashCategoryId,SupplierId,CashSubcategoryId")] CashMovement cashMovement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cashMovement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashMovement.CashCategoryId);
            ViewData["CashSubcategoryId"] = new SelectList(_context.CashSubcategory, "CashSubcategoryId", "CashSubcategoryDescription", cashMovement.CashSubcategoryId);
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription", cashMovement.CashMovementTypeId);
            ViewData["PaymentMediaId"] = new SelectList(_context.Set<PaymentMedia>(), "PaymentMediaId", "PaymentMediaDescription", cashMovement.PaymentMediaId);
            ViewData["SupplierId"] = new SelectList(_context.Set<Supplier>(), "SupplierId", "SupplierDescription", cashMovement.SupplierId);
            return View(cashMovement);
        }

        // GET: CashMovement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashMovement = await _context.CashMovement.FindAsync(id);
            if (cashMovement == null)
            {
                return NotFound();
            }
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory.Where(x => x.CashCategoryDescription != "Movimiento de pago"), "CashCategoryId", "CashCategoryDescription", cashMovement.CashCategoryId);
            ViewData["CashSubcategoryId"] = new SelectList(_context.CashSubcategory.Where(x => x.CashSubcategoryDescription != "Movimiento de pago"), "CashSubcategoryId", "CashSubcategoryDescription", cashMovement.CashSubcategoryId);
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription", cashMovement.CashMovementTypeId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier.Where(x => x.SupplierDescription != "Movimiento de pago"), "SupplierId", "SupplierDescription", cashMovement.SupplierId);
            ViewData["PaymentMediaId"] = new SelectList(_context.Set<PaymentMedia>(), "PaymentMediaId", "PaymentMediaDescription", cashMovement.PaymentMediaId);
            return View(cashMovement);
        }

        // POST: CashMovement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CashMovementId,CashMovementDate,CashMovementDetails,Amount,CashMovementTypeId,CashCategoryId,SupplierId,CashSubcategoryId,PaymentMediaId")] CashMovement cashMovement)
        {
            if (id != cashMovement.CashMovementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cashMovement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashMovementExists(cashMovement.CashMovementId))
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
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashMovement.CashCategoryId);
            ViewData["CashSubcategoryId"] = new SelectList(_context.CashSubcategory, "CashSubcategoryId", "CashSubcategoryDescription", cashMovement.CashSubcategoryId);
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription", cashMovement.CashMovementTypeId);
            ViewData["SupplierId"] = new SelectList(_context.Set<Supplier>(), "SupplierId", "SupplierDescription", cashMovement.SupplierId);
            ViewData["PaymentMediaId"] = new SelectList(_context.Set<PaymentMedia>(), "PaymentMediaId", "PaymentMediaDescription", cashMovement.PaymentMediaId);
            return View(cashMovement);
        }

        // GET: CashMovement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashMovement = await _context.CashMovement
                .Include(c => c.CashCategory)
                .Include(c => c.CashSubcategory)
                .Include(c => c.CashMovementType)
                .Include(c => c.Supplier)
                .Include(c => c.PaymentMedia)
                .FirstOrDefaultAsync(m => m.CashMovementId == id);
            if (cashMovement == null)
            {
                return NotFound();
            }

            return View(cashMovement);
        }

        // POST: CashMovement/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cashMovement = await _context.CashMovement.FindAsync(id);
            _context.CashMovement.Remove(cashMovement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public JsonResult ReturnJsonSubCategories(int categoryId)
        {
            var jsonData = _context.CashSubcategory.Where(x => x.CashCategoryId == categoryId).ToList();
            return Json(jsonData);
        }

        private bool CashMovementExists(int id)
        {
            return _context.CashMovement.Any(e => e.CashMovementId == id);
        }
    }
}
