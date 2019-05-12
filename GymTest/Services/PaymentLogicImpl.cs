using System;
using GymTest.Data;
using GymTest.Models;
using Microsoft.Extensions.Logging;

namespace GymTest.Services
{
    public class PaymentLogicImpl : IPaymentLogic
    {
        private readonly ISendEmail _sendEmail;

        private readonly GymTestContext _context;

        private readonly ILogger<IPaymentLogic> _logger;

        public PaymentLogicImpl(GymTestContext context, ISendEmail sendEmail, ILogger<IPaymentLogic> logger)
        {
            _logger = logger;
            _context = context;
            _sendEmail = sendEmail;
        }

        public bool ProcessPayment(Payment payment, string userName, string userEmail)
        {
            var isEdit = false;
            if (payment.PaymentId > 0)
                isEdit = true;

            try
            {
                _context.Add(payment);
                _context.SaveChangesAsync();
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
