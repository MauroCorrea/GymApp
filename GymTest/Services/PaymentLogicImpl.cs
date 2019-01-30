using System;
using GymTest.Data;
using GymTest.Models;

namespace GymTest.Services
{
    public class PaymentLogicImpl : IPaymentLogic
    {
        private readonly ISendEmail _sendEmail;

        private readonly GymTestContext _context;

        public PaymentLogicImpl(GymTestContext context, ISendEmail sendEmail)
        {
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
                var message = ex.Message;
                //log error
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
                var message = ex.Message;
                //log error
            }

            return true;
        }


    }
}
