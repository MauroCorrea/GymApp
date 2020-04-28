using System;
using GymTest.Data;
using GymTest.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GymTest.Services
{
    public class PaymentLogicImpl : IPaymentLogic
    {
        private readonly ISendEmail _sendEmail;

        private readonly GymTestContext _context;

        private readonly ILogger<IPaymentLogic> _logger;

        private readonly ITimezoneLogic _timeZone;

        public PaymentLogicImpl(GymTestContext context, ISendEmail sendEmail, ILogger<IPaymentLogic> logger, ITimezoneLogic timeZone)
        {
            _logger = logger;
            _context = context;
            _sendEmail = sendEmail;
            _timeZone = timeZone;
        }

        public bool HasPaymentValid(int userId)
        {
            var users = from m in _context.User
                        select m;

            users = users.Where(s => s.UserId == userId);

            if (users.Count() == 1)
            {
                var user = users.First();

                var payments = from m in _context.Payment
                               select m;

                payments = payments.Where(p => p.UserId == userId);

                if (payments.Count() > 0)
                {
                    var newestPayment = payments.OrderByDescending(p => p.PaymentDate).First();
                    if (newestPayment.MovementTypeId > 0)
                    {
                        if (newestPayment.LimitUsableDate.Date < _timeZone.GetCurrentDateTime(DateTime.Now).Date)
                        {
                            return false; // se venció el tiempo de uso del ultimo pago
                        }

                        switch (newestPayment.MovementTypeId)
                        {
                            #region Mensual
                            case (int)PaymentTypeEnum.Monthly:
                                var monthsPayed = newestPayment.QuantityMovmentType;

                                var monthsUsed = _timeZone.GetCurrentDateTime(DateTime.Now).Month - newestPayment.PaymentDate.Month;

                                if (_timeZone.GetCurrentDateTime(DateTime.Now).Year > newestPayment.PaymentDate.Year)
                                    monthsUsed += 12;

                                if (monthsUsed > monthsPayed)
                                {
                                    return false; // el pago actual ya no es válido
                                }

                                break;
                            #endregion
                            #region Por asistencias
                            case (int)PaymentTypeEnum.ByAssistances:
                                var userAssistance = from a in _context.Assistance select a;

                                userAssistance = userAssistance.Where(a => a.UserId == userId &&
                                                a.AssistanceDate.Date >= newestPayment.PaymentDate.Date);

                                if (userAssistance.Count() >= newestPayment.QuantityMovmentType)
                                {
                                    return false; // ya se consumieron todas las asistencias
                                }

                                break;
                            #endregion

                            default:
                                return false;//formato de pago inválido
                        }

                        //el usuario existe y tiene un pago válido
                        return true;
                    }
                    return false; //error: ultimo pago sin tipo de membresía

                }
                return false; //error: usuario sin pagos


            }
            else if (users.Count() > 1)
            {
                //error: solo 1 usuario deber identificado por id
                return false;
            }

            return false;
        }

        public bool ProcessPayment(Payment payment, string userName, string userEmail)
        {
            var isEdit = false;
            if (payment.PaymentId > 0)
                isEdit = true;

            try
            {
                if (isEdit)
                {
                    _context.Update(payment);
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(payment);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error Processing Payment. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error Processing Payment. Detail: " + ex.InnerException.Message);
                return false;
            }

            try
            {


                var bodyData = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "UserName", userName },
                    { "Title", "Notificación de pago realizado." },
                    { "amount", payment.Amount.ToString() },
                    { "paymentDate", payment.PaymentDate.ToShortDateString() },
                    { "limitDate", payment.LimitUsableDate.ToShortDateString() },
                    { "quantity", payment.QuantityMovmentType.ToString() }
                };

                switch (payment.MovementTypeId)
                {
                    #region Mensual
                    case (int)PaymentTypeEnum.Monthly:
                        if (payment.QuantityMovmentType > 1)
                            bodyData.Add("type", "meses");
                        else
                            bodyData.Add("type", "mes");
                        break;
                    #endregion
                    #region Por asistencias
                    case (int)PaymentTypeEnum.ByAssistances:
                        if (payment.QuantityMovmentType > 1)
                            bodyData.Add("type", "asistencias");
                        else
                            bodyData.Add("type", "asistencia");
                        break;
                        #endregion
                }

                if (isEdit)
                {
                    bodyData.Add("action", "editado");
                    bodyData.Add("paymentId", payment.PaymentId + " ");
                }
                else
                {
                    bodyData.Add("action", "registrado");
                    bodyData.Add("paymentId", string.Empty);
                }

                _sendEmail.SendEmail(bodyData,
                                     "PaymentTemplate",
                                     "Notificación de pago realizado",
                                     new System.Collections.Generic.List<string>() { userEmail }
                                    );
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error Sending email. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error Sending email. Detail: " + ex.InnerException.Message);
            }

            return true;
        }


    }
}
