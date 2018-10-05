using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using GymTest.Data;

namespace GymTest.Models
{
    public class SeedData
    {
        IConfiguration _iconfiguration;

        public SeedData(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new GymTestContext(
                serviceProvider.GetRequiredService<DbContextOptions<GymTestContext>>()))
            {
                // Look for any movies.
                if (true) // levantar la info del appsettings
                {
                    return;   // DB has been seeded
                }

                //context.CashCategories.AddRange(
                //    new CashCategory
                //     {
                //    CashCategoryDescription = "Categoria 1",
                //    CashCategoryId = 1
                //     }, 
                //    new CashCategory
                //     {
                //    CashCategoryDescription = "Categoria 2",
                //    CashCategoryId = 2
                //     }
                //);
                context.SaveChanges();
            }
        }
    }
}