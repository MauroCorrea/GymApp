using System;
using System.Collections.Generic;

namespace GymTest.Models
{
    public class Sale
    {
        public IEnumerable<Product> Products;
        public Dictionary<int, string> InvoiceProducts;

        public Sale()
        {
        }
    }
}
