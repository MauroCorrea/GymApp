using GymTest.Data;
using GymTest.Models;
using System.Linq;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.Logging;

namespace GymTest.Services
{
    public class PaymentNotificationLogicImpl : IPaymentNotificationLogic
    {

        private readonly GymTestContext _context;

        private readonly ISendEmail _sendEmail;

        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        private readonly ILogger<IPaymentLogic> _logger;


        public PaymentNotificationLogicImpl(GymTestContext context, IOptionsSnapshot<AppSettings> app, ISendEmail sendEmail, ILogger<IPaymentLogic> logger)
        {
            _appSettings = app;
            _context = context;
            _sendEmail = sendEmail;
        }

        public void NotifyUsers()
        {
            try
            {
                AutomaticProcess automaticSendMailProcess = _context.AutomaticProcess.
                    Where(x => x.AutomaticProcessId == int.Parse(_appSettings.Value.PaymentNotificationProcessId)).FirstOrDefault();
                if (automaticSendMailProcess != null && automaticSendMailProcess.NextProcessDate <= DateTime.Today)
                {

                    string notifyByDate = _appSettings.Value.PaymentNotificationByDate;
                    if (bool.Parse(notifyByDate))
                    {

                        NotifyByDate();

                    }

                    string notifyByExp = _appSettings.Value.PaymentNotificationByExpiration;
                    if (bool.Parse(notifyByExp))
                    {
                        NotifyByExpiration();
                    }

                    automaticSendMailProcess.NextProcessDate = DateTime.Now.Date.AddDays(int.Parse(_appSettings.Value.PaymentNotificationProcessAddDays));

                    _context.Update(automaticSendMailProcess);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error Processing Payment. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error Processing Payment. Detail: " + ex.InnerException.Message);
            }
        }

        private void NotifyByDate()
        {
            var users = from m in _context.User
                        select m;

            var sendMail = false;
            var message = string.Empty;

            foreach (var user in users)
            {
                sendMail = false;

                var payments = from m in _context.Payment
                               select m;
                payments = payments.Where(p => p.UserId == user.UserId);

                if (payments.Count() > 0)
                {
                    var newestPayment = payments.OrderByDescending(p => p.PaymentDate).First();
                    if (newestPayment.MovementTypeId > 0)
                    {
                        var dateMinToNotify = newestPayment.LimitUsableDate.Date.AddDays(-int.Parse(_appSettings.Value.PaymentNotificationDaysBefore));
                        if (DateTime.Now.Date >= dateMinToNotify)
                        {
                            // se venció el tiempo de uso del ultimo pago
                            message = "Fecha límite de uso cercana o sobrepasada. La misma es " + newestPayment.LimitUsableDate.Date.ToShortDateString() + ".";
                            sendMail = true;
                        }
                    }
                }
                else//error: usuario sin pagos
                {
                    message = "Usuario sin pagos.";
                    sendMail = true;
                }
                if (sendMail)
                {
                    var bodyData = new System.Collections.Generic.Dictionary<string, string>
                        {
                            { "UserName", user.FullName },
                            { "Message", message }
                        };

                    _sendEmail.SendEmail(bodyData,
                                            "AssistanceTemplateFinishPayment",
                                            "Notificación por expiración de acceso",
                                            new System.Collections.Generic.List<string>() { user.Email }
                                        );
                }

            }
        }

        private void NotifyByExpiration()
        {
            var users = from m in _context.User
                        select m;

            var sendMail = false;
            var message = string.Empty;

            foreach (var user in users)
            {
                sendMail = false;

                var payments = from m in _context.Payment
                               select m;
                payments = payments.Where(p => p.UserId == user.UserId);

                if (payments.Count() > 0)
                {
                    var newestPayment = payments.OrderByDescending(p => p.PaymentDate).First();
                    if (newestPayment.MovementTypeId > 0)
                    {
                        if (newestPayment.LimitUsableDate.Date < DateTime.Now.Date)
                        {
                            // se venció el tiempo de uso del ultimo pago
                            message = "Fecha límite de uso sobrepasada. La misma es " + newestPayment.LimitUsableDate.Date.ToShortDateString() + ".";
                            sendMail = true;
                        }

                        switch (newestPayment.MovementTypeId)
                        {
                            #region Mensual
                            case (int)PaymentTypeEnum.Monthly:
                                var monthsPayed = newestPayment.QuantityMovmentType;

                                var monthsUsed = DateTime.Now.Month - newestPayment.PaymentDate.Month;

                                if (DateTime.Now.Year > newestPayment.PaymentDate.Year)
                                    monthsUsed += 12;

                                if (monthsUsed > monthsPayed)
                                {
                                    //Ya pasó el mes
                                    message = "Pago mensual vencido. Su último fue por " + monthsPayed + " mes(es) el día " +
                                        newestPayment.PaymentDate.ToString("dd/MM/yyyy HH:mm")
                                        + " y se utilizaron " + monthsUsed + " mes(es).";
                                    sendMail = true;
                                }

                                var dayMinToNotify = int.Parse(_appSettings.Value.PaymentNotificationDayToPay) -
                                                    int.Parse(_appSettings.Value.PaymentNotificationDaysBefore);

                                //Si estamos en mes vencido, tenemos que ver la fecha.
                                if (monthsUsed == monthsPayed && DateTime.Now.Day >= dayMinToNotify)
                                {
                                    message = "Pago mensual está por vencer. Su último fue por " + monthsPayed + " mes(es) el día " +
                                        newestPayment.PaymentDate.ToString("dd/MM/yyyy HH:mm")
                                        + " y se utilizaron " + monthsUsed + " mes(es).";
                                    sendMail = true;
                                }

                                break;
                            #endregion
                            #region Por asistencias
                            case (int)PaymentTypeEnum.ByAssistances:
                                var userAssistances = from a in _context.Assistance select a;

                                userAssistances = userAssistances.Where(a => a.UserId == user.UserId &&
                                                a.AssistanceDate.Date >= newestPayment.PaymentDate.Date);

                                if (userAssistances.Count() >= newestPayment.QuantityMovmentType)
                                {
                                    // ya se consumieron todas las asistencias
                                    message = "Pago por asistencias consumido. Se habilitaron " + newestPayment.QuantityMovmentType + " asistencia(s) y se utilizaron " + userAssistances.Count() + " asistencia(s).";
                                    sendMail = true;
                                }
                                else if (userAssistances.Count() >= newestPayment.QuantityMovmentType - int.Parse(_appSettings.Value.PaymentNotificationAssitanceBefore))
                                {
                                    // le avisamos antes
                                    message = "Pago por asistencias pronto a consumirse. Se habilitaron " + newestPayment.QuantityMovmentType + " asistencia(s) y se utilizaron " + userAssistances.Count() + " asistencia(s).";
                                    sendMail = true;
                                }
                                break;
                            #endregion

                            default:
                                break;
                        }

                    }
                    else//error: ultimo pago sin tipo de membresía
                    {
                        message = "Último pago sin tipo de membresía.";
                        sendMail = true;
                    }
                }
                else//error: usuario sin pagos
                {
                    message = "Usuario sin pagos.";
                    sendMail = true;
                }
                if (sendMail)
                {
                    var bodyData = new System.Collections.Generic.Dictionary<string, string>
                        {
                            { "UserName", user.FullName },
                            { "Message", message }
                        };

                    _sendEmail.SendEmail(bodyData,
                                            "AssistanceTemplateFinishPayment",
                                            "Notificación por expiración de acceso",
                                            new System.Collections.Generic.List<string>() { user.Email }
                                        );
                }
            }
        }
    }
}