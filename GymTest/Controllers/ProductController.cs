using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymTest.Data;
using GymTest.Models;

namespace GymTest.Controllers
{
    public class ProductController : Controller
    {
        private readonly GymTestContext _context;
        private static Dictionary<Product, int> _invoice = new Dictionary<Product, int>();

        public ProductController(GymTestContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        public async Task<IActionResult> Sale(int? productId, bool isDelete = false)
        {
            if (isDelete)
            {
                DeleteProductToInvoice((int)productId);
            }
            else
            {
                if (productId != null)
                {
                    LoadProductToInvoice((int)productId);
                }
                else
                {
                    _invoice = new Dictionary<Product, int>();
                }
            }
            
            var sale = new Sale();
            sale.Products = _context.Product.ToList();
            sale.InvoiceProducts = GetInvoiceProduct();

            return View(sale);
        }

        private void DeleteProductToInvoice(int productId)
        {
            var product = _invoice.Keys.Where(prodKey => prodKey.ProductId == productId).First();
            _invoice.Remove(product);
        }

        private Dictionary<int, string> GetInvoiceProduct()
        {
            var result = new Dictionary<int, string>();
            double total = 0;

            foreach(var product in _invoice.Keys)
            {
                var productQuantity = _invoice[product];
                var subTotal = (product.ProductPrice * productQuantity);
                var line = product.ProductDescription
                    + " x"
                    + productQuantity.ToString()
                    + "    $"
                    + subTotal.ToString();
                result.Add(product.ProductId, line);
                total += subTotal;
            }

            var lastLine = "TOTAL:     $" + total.ToString();
            result.Add(0, lastLine);
            return result;
        }

        private void LoadProductToInvoice(int productId)
        {
            var product = _invoice.Keys.Where(prodKey => prodKey.ProductId == productId).FirstOrDefault();
            if (product != null)
            {
                _invoice[product] = _invoice[product] + 1;
            }
            else
            {
                product = _context.Product.Find(productId);
                _invoice.Add(product, 1);
            }
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductDescription,ProductPrice,ProductQuantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,ProductPrice,ProductQuantity")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> CancelPayment()
        {
            return RedirectToAction(nameof(Sale));
        }

        public async Task<IActionResult> CreatePayment(string totalLine)
        {
            if (!String.IsNullOrEmpty(totalLine))
            {
                var amount = Convert.ToDouble(totalLine.Split("$")[1]);
                if(amount > 0)
                {
                    var detailInvoice = GetDetailInvoiceAndUpdateStock();
                    CashMovement cashMov = new CashMovement();

                    cashMov.Amount = amount;
                    cashMov.PaymentMediaId = 1;
                    cashMov.CashMovementDate = DateTime.Now;
                    cashMov.CashMovementDetails = detailInvoice;
                    cashMov.CashMovementTypeId = 1;//1 es de tipo entrada
                    cashMov.CashCategoryId = _context.CashCategory.Where(x => x.CashCategoryDescription == "Venta").FirstOrDefault().CashCategoryId;
                    cashMov.CashSubcategoryId = _context.CashSubcategory.Where(x => x.CashSubcategoryDescription == "Venta").FirstOrDefault().CashSubcategoryId;
                    cashMov.SupplierId = _context.Supplier.Where(x => x.SupplierDescription == "Venta").FirstOrDefault().SupplierId;
                    cashMov.PaymentId = null;
                    
                    _context.CashMovement.Add(cashMov);
                    _context.SaveChanges();
                }
            }
            
            return RedirectToAction(nameof(Sale));
        }

        private string GetDetailInvoiceAndUpdateStock()
        {
            var result = String.Empty;
            foreach(var invoiceElement in _invoice)
            {
                var product = invoiceElement.Key;
                product.ProductQuantity -= invoiceElement.Value;

                if (!String.IsNullOrEmpty(result))
                {
                    result = result + ", ";
                }
                result = result + product.ProductDescription + " x" + invoiceElement.Value.ToString();

                _context.Product.Update(product);
            }
            _context.SaveChanges();
            return result;
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
