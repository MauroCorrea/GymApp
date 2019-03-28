using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTest.Models;
using GymTest.Data;

using OfficeOpenXml;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;

namespace GymTest.Controllers
{
    public class CashMovementController : Controller
    {
        private readonly GymTestContext _context;

        public CashMovementController(GymTestContext context)
        {
            _context = context;
        }

        // GET: CashMovement
        public async Task<IActionResult> Index()
        {
            var gymTestContext = _context.CashMovement
                                         .Include(c => c.CashCategory)
                                         .Include(c => c.CashMovementType)
                                         .Include(c => c.Supplier);
           return View(await gymTestContext.ToListAsync());
        }

        public IActionResult ExportToExcel()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string Ruta_Publica_Excel = (path + "/MovimientosDeCaja_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx");

            ExcelPackage Package = new ExcelPackage(new System.IO.FileInfo(Ruta_Publica_Excel));
            var Hoja_1 = Package.Workbook.Worksheets.Add("Contenido_1");

            /*------------------------------------------------------*/
            int rowNum = 2;
            int originalRowNum = rowNum;

            Hoja_1.Cells["B" + rowNum].Value = "Detalles";
            Hoja_1.Cells["C" + rowNum].Value = "Tipo";
            Hoja_1.Cells["D" + rowNum].Value = "Categoría";
            Hoja_1.Cells["E" + rowNum].Value = "Fecha";
            Hoja_1.Cells["F" + rowNum].Value = "Monto";
            Hoja_1.Cells["G" + rowNum].Value = "Proveedor";

            Hoja_1.Cells["B" + rowNum + ":G" + rowNum].Style.Font.Bold = true;
            Hoja_1.Cells["B" + rowNum + ":G" + rowNum].Style.Font.Size = 15;

            foreach (CashMovement row in _context.CashMovement)
            {
                row.CashMovementType = _context.CashMovementType.Where(x => x.CashMovementTypeId == row.CashMovementTypeId).First();
                row.CashCategory = _context.CashCategory.Where(x => x.CashCategoryId == row.CashCategoryId).First();
                row.Supplier = _context.Supplier.Where(x => x.SupplierId == row.SupplierId).First();

                rowNum++;
                Hoja_1.Cells["B" + rowNum].Value = row.CashMovementDetails;
                Hoja_1.Cells["C" + rowNum].Value = row.CashMovementType.CashMovementTypeDescription;
                Hoja_1.Cells["D" + rowNum].Value = row.CashCategory.CashCategoryDescription;
                Hoja_1.Cells["E" + rowNum].Value = row.CashMovementDate.ToString();
                Hoja_1.Cells["F" + rowNum].Value = row.CashMovementTypeId == 1 ? row.Amount : (row.Amount * (-1));
                Hoja_1.Cells["G" + rowNum].Value = row.Supplier.SupplierDescription;
            }

            Hoja_1.Cells["F" + (rowNum + 1)].Formula = "SUM(F" + (originalRowNum + 1) + ":F" + rowNum + ")";


            /*------------------------------------------------------*/

            Package.Save();

            /*------------------------------------------------------*/
            /*Response.ContentType = "application/force-download";
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentType = "application/download";

          
            Response.Headers.Add("content-disposition", string.Format("attachment;  filename={0}", "ExcelWeb.xlsx"));
            await Response.Body.WriteAsync(Package.GetAsByteArray());
            await Response.Body.FlushAsync();*/
            /*------------------------------------------------------*/

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
                .Include(c => c.CashMovementType)
                .Include(c => c.Supplier)
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
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription");
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription");
            ViewData["SupplierId"] = new SelectList(_context.Set<Supplier>(), "SupplierId", "SupplierDescription");
            return View();
        }

        // POST: CashMovement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CashMovementId,CashMovementDate,CashMovementDetails,Amount,CashMovementTypeId,CashCategoryId,SupplierId")] CashMovement cashMovement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cashMovement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashMovement.CashCategoryId);
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription", cashMovement.CashMovementTypeId);
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
            ViewData["CashCategoryId"] = new SelectList(_context.CashCategory, "CashCategoryId", "CashCategoryDescription", cashMovement.CashCategoryId);
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription", cashMovement.CashMovementTypeId);
            ViewData["SupplierId"] = new SelectList(_context.Set<Supplier>(), "SupplierId", "SupplierDescription", cashMovement.SupplierId);
            return View(cashMovement);
        }

        // POST: CashMovement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CashMovementId,CashMovementDate,CashMovementDetails,Amount,CashMovementTypeId,CashCategoryId,SupplierId")] CashMovement cashMovement)
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
            ViewData["CashMovementTypeId"] = new SelectList(_context.Set<CashMovementType>(), "CashMovementTypeId", "CashMovementTypeDescription", cashMovement.CashMovementTypeId);
            ViewData["SupplierId"] = new SelectList(_context.Set<Supplier>(), "SupplierId", "SupplierDescription", cashMovement.SupplierId);
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
                .Include(c => c.CashMovementType)
                .Include(c => c.Supplier)
                .FirstOrDefaultAsync(m => m.CashMovementId == id);
            if (cashMovement == null)
            {
                return NotFound();
            }

            return View(cashMovement);
        }

        // POST: CashMovement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cashMovement = await _context.CashMovement.FindAsync(id);
            _context.CashMovement.Remove(cashMovement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CashMovementExists(int id)
        {
            return _context.CashMovement.Any(e => e.CashMovementId == id);
        }
    }
}
