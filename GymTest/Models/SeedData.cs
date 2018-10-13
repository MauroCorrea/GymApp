using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
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
                //if (true) // levantar la info del appsettings
                //{
                //    return;   // DB has been seeded
                //}

                var moveTypes = from m in context.MovementType
                                select m;

                if (moveTypes.Count() != 2)
                {
                    context.MovementType.AddRange(
                        new MovementType
                        {
                            Description = "Mensual",
                            MovementTypeId = 1
                        },
                        new MovementType
                        {
                            Description = "Por asistencia",
                            MovementTypeId = 2
                        }
                    );
                }

                context.SaveChanges();
            }
        }
    }
}