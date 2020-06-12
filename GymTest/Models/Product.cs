using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GymTest.Models
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class Product
    {
        public int ProductId { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Campo nombre de producto es obligatorio")]
        public string ProductName { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "Campo descripción de producto es obligatorio")]
        public string ProductDescription { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "Campo precio de producto es obligatorio")]
        [DisplayFormat(DataFormatString = "$ {0:0.00}")]
        public double ProductPrice { get; set; }

        [Display(Name = "Cantidad")]
        public int ProductQuantity { get; set; }



        public Product()
        {
        }
    }
}
