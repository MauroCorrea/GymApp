using System;
using System.Linq;
using GymTest.Data;
using GymTest.Models;

namespace GymTest.Services
{
    public class AssistanceLogicImpl : IAssistanceLogic
    {

        private readonly GymTestContext _context;

        public AssistanceLogicImpl(GymTestContext context)
        {
            _context = context;
        }

        public bool ProcessAssistance(string userToken)
        {
            var objectToReturn = false;
            var users = from m in _context.User
                        select m;

            users = users.Where(s => s.Email.ToLower().Equals(userToken.ToLower()));

            if (users.Count() == 1)
            {
                var userid = users.First().UserId;

                var payments = from m in _context.Payment
                               select m;
                payments = payments.Where(p => p.UserId == userid);

                if (payments.Count() > 0)
                {
                    var newestPayment = payments.OrderByDescending(p => p.PaymentDate).First();
                    if (newestPayment.MovmentTypeId > 0)
                    {
                        switch (newestPayment.MovmentTypeId)
                        {
                            case (int)PaymentTypeEnum.Monthly:
                                var monthsPayed = newestPayment.QuantityMovmentType;

                                var monthsUsed = DateTime.Now.Month - newestPayment.PaymentDate.Month;

                                if (DateTime.Now.Year > newestPayment.PaymentDate.Year)
                                    monthsUsed += 12;

                                if (monthsUsed > monthsPayed)
                                    return objectToReturn;

                                break;
                            case (int)PaymentTypeEnum.ByAssistances:
                                var ass = from a in _context.Assistance select a;

                                ass = ass.Where(a => a.UserId == userid &&
                                                a.AssistanceDate.Date >= newestPayment.PaymentDate.Date);

                                if (ass.Count() >= newestPayment.QuantityMovmentType)
                                    return objectToReturn;
                                break;
                            default:
                                return objectToReturn;
                        }
                    }
                    else//error: ultimo pago sin tipo de membresía
                    {
                        return objectToReturn;

                    }
                }
                else //error: usuario sin pagos
                {
                    return objectToReturn;
                }

            }
            else
            {
                //error: solo 1 usuario deber identificado por token
                return objectToReturn;
            }
            //TODO: Creamos asistencia en caso de devolver solo true? que pasa si aunque sea false, el usuario le permiten entrenar/bailar?
            //Assistance assistance = new Assistance();
            //assistance.User = users.FirstOrDefault();
            //assistance.AssistanceDate = DateTime.Now;
            //_context.Assistance.Add(assistance);
            //_context.SaveChangesAsync();

            return true;
        }
    }
}
