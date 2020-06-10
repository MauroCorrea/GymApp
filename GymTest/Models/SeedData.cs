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

                LoadMoveTypes(context);

                LoadPaymentMedia(context);

                LoadCashCategories(context);

                LoadCashSubcategories(context);

                LoadSuppliers(context);

                LoadCashMovementTypes(context);

                LoadMedicalEmergencies(context);

                context.SaveChanges();
            }
        }

        private static void LoadMedicalEmergencies(GymTestContext context)
        {
            var list = from m in context.MedicalEmergency
                       select m;

            if (list.Count() <= 0)
            {
                context.MedicalEmergency.AddRange(
                    new MedicalEmergency
                    {
                        MedicalEmergencyDescription = "SEMM",
                        MedicalEmergencyId = 1
                    },
                    new MedicalEmergency
                    {
                        MedicalEmergencyDescription = "SUAT",
                        MedicalEmergencyId = 2
                    }
                );
            }
        }

        private static void LoadMoveTypes(GymTestContext context)
        {
            var list = from m in context.MovementType
                       select m;

            if (list.Count() != 2)
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
        }

        private static void LoadPaymentMedia(GymTestContext context)
        {
            var list = from m in context.PaymentMedia
                       select m;

            if (list.Count() < 1)
            {
                context.PaymentMedia.AddRange(
                    new PaymentMedia
                    {
                        PaymentMediaId = 1,
                        PaymentMediaDescription = "Efectivo"
                    },
                    new PaymentMedia
                    {
                        PaymentMediaId = 2,
                        PaymentMediaDescription = "Débito"
                    },
                    new PaymentMedia
                    {
                        PaymentMediaId = 3,
                        PaymentMediaDescription = "Crédito"
                    },
                    new PaymentMedia
                    {
                        PaymentMediaId = 4,
                        PaymentMediaDescription = "Tranferencia"
                    }
                );
            }
        }

        private static void LoadUsers(GymTestContext context)
        {
            var list = from m in context.User
                       select m;

            if (list.Count() < 1)
            {
                context.User.AddRange(
                    new User
                    {
                        Address = "Direccion por defecto",
                        BirthDate = new DateTime(1988, 01, 28),
                        Commentaries = "Estos son los comentarios",
                        DocumentNumber = "123456789",
                        Email = "mauro.correa1988@gmail.com",
                        FullName = "Nombre Apellido",
                        Phones = "099123456",
                        SignInDate = DateTime.Now,
                        Token = "123123",
                        ContactFullName = "Seba Perez",
                        ContactPhones = "220098765",
                        HealthCronicalProblems = "Ninguno",
                        HealthHeartProblems = "Ninguno",
                        HealthPhysicalProblems = "Ninguno",
                        HealthRegularPills = "Ninguno"
                    }
                );
            }
        }

        private static void LoadCashSubcategories(GymTestContext context)
        {
            var list = from m in context.CashSubcategory
                       select m;

            if (list.Count() < 1)
            {
                context.CashSubcategory.AddRange(
                    new CashSubcategory
                    {
                        CashCategoryId = 1,
                        CashSubcategoryDescription = "Otros"
                    },
                    new CashSubcategory
                    {
                        CashCategoryId = 2,
                        CashSubcategoryDescription = "Reserva Cancha"
                    },
                    new CashSubcategory
                    {
                        CashCategoryId = 3,
                        CashSubcategoryDescription = "Venta"
                    }
                );
            }
        }

        private static void LoadCashCategories(GymTestContext context)
        {
            var list = from m in context.CashCategory
                       select m;

            if (list.Count() < 1)
            {
                context.CashCategory.AddRange(
                    new CashCategory
                    {
                        CashCategoryId = 1,
                        CashCategoryDescription = "Otros"
                    },
                    new CashCategory
                    {
                        CashCategoryId = 2,
                        CashCategoryDescription = "Reserva Cancha"
                    },
                    new CashCategory
                    {
                        CashCategoryId = 3,
                        CashCategoryDescription = "Venta"
                    }
                );
            }
        }

        private static void LoadCashMovementTypes(GymTestContext context)
        {
            var list = from m in context.CashMovementType
                       select m;

            if (list.Count() < 1)
            {
                context.CashMovementType.AddRange(
                    new CashMovementType
                    {
                        CashMovementTypeDescription = "Entrada"
                    },
                    new CashMovementType
                    {
                        CashMovementTypeDescription = "Salida"
                    }
                );
            }
        }

        private static void LoadSuppliers(GymTestContext context)
        {
            var list = from m in context.Supplier
                       select m;

            if (list.Count() < 1)
            {
                context.Supplier.AddRange(
                    new Supplier
                    {
                        SupplierId = 1,
                        SupplierDescription = "Proveedor 1"
                    },
                    new Supplier
                    {
                        SupplierId = 2,
                        SupplierDescription = "Reserva Cancha"
                    },
                    new Supplier
                    {
                        SupplierId = 3,
                        SupplierDescription = "Venta"
                    }
                );
            }
        }
    }
}