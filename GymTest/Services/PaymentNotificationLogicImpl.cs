using GymTest.Data;
using GymTest.Models;
using System.Linq;
using Microsoft.Extensions.Options;
using System;

namespace GymTest.Services
{
    public class PaymentNotificationLogicImpl : IPaymentNotificationLogic
    {

        private readonly GymTestContext _context;

        private readonly ISendEmail _sendEmail;

        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        public PaymentNotificationLogicImpl(GymTestContext context, IOptionsSnapshot<AppSettings> app, ISendEmail sendEmail)
        {
            _appSettings = app;
            _context = context;
            _sendEmail = sendEmail;
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
                        if (newestPayment.LimitUsableDate.Date < DateTime.Now.Date)
                        {
                            // se venció el tiempo de uso del ultimo pago
                            message = "Fecha límite de uso sobrepasada. La misma es " + newestPayment.LimitUsableDate.Date.ToShortDateString() + ".";
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
                    //Todo: send mail to user with message
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
                                    message = "Pago mensual vencido. Su último fue por " + monthsPayed + " mes(es) y se utilizaron " + monthsUsed + " mes(es).";
                                    sendMail = true;
                                }

                                break;
                            #endregion
                            #region Por asistencias
                            case (int)PaymentTypeEnum.ByAssistances:
                                var ass = from a in _context.Assistance select a;

                                ass = ass.Where(a => a.UserId == user.UserId &&
                                                a.AssistanceDate.Date >= newestPayment.PaymentDate.Date);

                                if (ass.Count() >= newestPayment.QuantityMovmentType)
                                {
                                    // ya se consumieron todas las asistencias
                                    message = "Pago por asistencias consumido. Se habilitaron " + newestPayment.QuantityMovmentType + " asistencia(s) y se utilizaron " + ass.Count() + " asistencia(s).";
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
                    //Todo: send mail to user with message
                }
            }
        }

        public void NotifyUsers()
        {
            AutomaticProcess automaticSendMailProcess = _context.AutomaticProcess.Where(x => x.AutomaticProcessId == int.Parse(_appSettings.Value.PaymentNotificationProcessId)).FirstOrDefault();
            if (automaticSendMailProcess != null && automaticSendMailProcess.NextProcessDate <= DateTime.Today)
            {

                string notifyByDate = _appSettings.Value.PaymentNotificationByDate;
                if (bool.Parse(notifyByDate))
                {
                    var dayOfMonth = _appSettings.Value.PaymentNotificationDayToPay;

                    if (dayOfMonth != null && int.Parse(dayOfMonth) > 0)
                    {
                        NotifyByDate();
                    }
                }

                string notifyByExp = _appSettings.Value.PaymentNotificationByExpiration;
                if (bool.Parse(notifyByExp))
                {
                    NotifyByExpiration();
                }

                automaticSendMailProcess.NextProcessDate.AddDays(int.Parse(_appSettings.Value.PaymentNotificationProcessAddDays));
                automaticSendMailProcess.NextProcessDate.AddMonths(int.Parse(_appSettings.Value.PaymentNotificationProcessAddMonths));

                _context.AutomaticProcess.Update(automaticSendMailProcess);
                _context.SaveChanges();
            }
        }
    }
}