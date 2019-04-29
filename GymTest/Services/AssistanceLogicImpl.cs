using System;
using System.Linq;
using GymTest.Data;
using GymTest.Models;
using Microsoft.Extensions.Options;

namespace GymTest.Services
{
    public class AssistanceLogicImpl : IAssistanceLogic
    {

        private readonly GymTestContext _context;

        private readonly ISendEmail _sendEmail;

        private readonly IOptions<AppSettings> _appSettings;

        public AssistanceLogicImpl(GymTestContext context, IOptions<AppSettings> app, ISendEmail sendEmail)
        {
            _appSettings = app;
            _context = context;
            _sendEmail = sendEmail;
        }

        public AssistanceInformation ProcessAssistance(string userToken, DateTime? assistanceDate = null)
        {
            var objectToReturn = new AssistanceInformation();
            var users = from m in _context.User
                        select m;

            users = users.Where(s => s.Token.ToLower().Equals(userToken.ToLower()));

            if (users.Count() == 1)
            {
                objectToReturn.User = users.First();

                var payments = from m in _context.Payment
                               select m;
                payments = payments.Where(p => p.UserId == objectToReturn.User.UserId);

                if (payments.Count() > 0)
                {
                    var newestPayment = payments.OrderByDescending(p => p.PaymentDate).First();
                    if (newestPayment.MovementTypeId > 0)
                    {
                        if (newestPayment.LimitUsableDate.Date < DateTime.Now.Date)
                        {
                            objectToReturn.Message = "Fecha límite de uso sobrepasada. La misma es " + newestPayment.LimitUsableDate.Date.ToShortDateString() + ".";
                            ProcessNotEntryNotification(objectToReturn.User.FullName, objectToReturn.Message, objectToReturn.User.Email,
                                newestPayment.PaymentDate.ToString("dd/MM/yyyy HH:mm"));
                            return objectToReturn; // se venció el tiempo de uso del ultimo pago
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
                                    objectToReturn.Message = "Pago mensual vencido. Su último fue por " + monthsPayed + " mes(es) y se utilizaron " + monthsUsed + " mes(es).";
                                    ProcessNotEntryNotification(objectToReturn.User.FullName, objectToReturn.Message, objectToReturn.User.Email,
                                        newestPayment.PaymentDate.ToString("dd/MM/yyyy HH:mm"));
                                    return objectToReturn; // el pago actual ya no es válido

                                }

                                objectToReturn.AdditionalData = "Su último pago fue por " + monthsPayed + " mes(es) y se utilizaron " + monthsUsed + " mes(es)."; ;
                                break;
                            #endregion
                            #region Por asistencias
                            case (int)PaymentTypeEnum.ByAssistances:
                                var ass = from a in _context.Assistance select a;

                                ass = ass.Where(a => a.UserId == objectToReturn.User.UserId &&
                                                a.AssistanceDate.Date >= newestPayment.PaymentDate.Date);

                                if (ass.Count() >= newestPayment.QuantityMovmentType)
                                {
                                    objectToReturn.Message = "Pago por asistencias consumido. Se habilitaron " + newestPayment.QuantityMovmentType + " asistencia(s) y se utilizaron " + ass.Count() + " asistencia(s).";
                                    ProcessNotEntryNotification(objectToReturn.User.FullName, objectToReturn.Message, objectToReturn.User.Email,
                                        newestPayment.PaymentDate.ToString("dd/MM/yyyy HH:mm"));
                                    return objectToReturn; // ya se consumieron todas las asistencias

                                }

                                objectToReturn.AdditionalData = "Cantidad de asistencias restantes: " + (newestPayment.QuantityMovmentType - ass.Count()) + ".";
                                break;
                            #endregion

                            default:
                                objectToReturn.Message = "Formato de pago no procesable.";
                                ProcessNotEntryNotification(objectToReturn.User.FullName, objectToReturn.Message, objectToReturn.User.Email,
                                    newestPayment.PaymentDate.ToString("dd/MM/yyyy HH:mm"));
                                return objectToReturn;
                        }

                        var lastAsistance = _context.Assistance.
                                                    Where(a => a.UserId == objectToReturn.User.UserId).
                                                    OrderByDescending(a => a.AssistanceDate).FirstOrDefault();

                        DateTime realAssistanceDate = assistanceDate.HasValue ? (DateTime)assistanceDate : DateTime.Now;

                        double diffTimeSecs = int.Parse(_appSettings.Value.AssistanceConfiguration_DiffHours) * 60 * 60
                            + int.Parse(_appSettings.Value.AssistanceConfiguration_DiffMins) * 60
                            + int.Parse(_appSettings.Value.AssistanceConfiguration_DiffSecs);

                        if (lastAsistance == null ||
                            (realAssistanceDate - lastAsistance.AssistanceDate).TotalSeconds > diffTimeSecs)
                        {
                            //Creamos asistencia en caso de que el usuario pueda entrar. Caso contrario, queda a criterio del lugar si pasa o no.
                            Assistance assistance = new Assistance
                            {
                                User = users.First(),
                                AssistanceDate = realAssistanceDate
                            };
                            _context.Assistance.Add(assistance);
                            _context.SaveChanges();

                            if (newestPayment.MovementTypeId == (int)PaymentTypeEnum.ByAssistances)
                            {
                                var ass = from a in _context.Assistance select a;

                                ass = ass.Where(a => a.UserId == objectToReturn.User.UserId &&
                                                a.AssistanceDate.Date >= newestPayment.PaymentDate.Date);
                                objectToReturn.AdditionalData = "Cantidad de asistencias restantes: " + (newestPayment.QuantityMovmentType - ass.Count()) + ".";
                            }

                            objectToReturn.Message = "Bienvenido. Disfrute de su jornada. Asistencia generada con fecha " + DateTime.Now + ".";

                            ProcessAssistanceNotification(objectToReturn.User.UserId);
                        }
                        else
                        {
                            objectToReturn.Message = "Bienvenido nuevamente. Su token ya fue ingresado " + (DateTime.Now - lastAsistance.AssistanceDate).TotalMinutes.ToString("0") + " minutos antes.";
                        }

                        objectToReturn.AdditionalData = objectToReturn.AdditionalData + "Fecha límite de uso del pago: " + newestPayment.LimitUsableDate.ToShortDateString() + ".";

                    }
                    else//error: ultimo pago sin tipo de membresía
                    {
                        objectToReturn.Message = "Último pago sin tipo de membresía.";
                        ProcessNotEntryNotification(objectToReturn.User.FullName, objectToReturn.Message, objectToReturn.User.Email,
                            newestPayment.PaymentDate.ToString("dd/MM/yyyy HH:mm"));

                        return objectToReturn;

                    }
                }
                else//error: usuario sin pagos
                {
                    objectToReturn.Message = "Usuario sin pagos.";
                    ProcessNotEntryNotification(objectToReturn.User.FullName, objectToReturn.Message, objectToReturn.User.Email, string.Empty);
                    return objectToReturn;
                }

            }
            else if (users.Count() > 1)
            {
                //error: solo 1 usuario deber identificado por token
                objectToReturn.Message = "Existen " + users.Count() + " usuarios registrados para el mismo token. Los usuarios son: ";
                foreach (var user in users)
                {
                    objectToReturn.Message = objectToReturn.Message + user.FullName + ",";
                }
                objectToReturn.Message.Remove(objectToReturn.Message.Length - 1);
                objectToReturn.Message = objectToReturn.Message + ".";
                return objectToReturn;
            }

            if (objectToReturn.User == null)
                objectToReturn.Message = "Usuario no encontrado.";

            return objectToReturn;
        }

        public void ProcessNotEntryNotification(string fullName, string details, string email, string lastPaymentDate)
        {
            if (!string.IsNullOrEmpty(lastPaymentDate))
                lastPaymentDate = "Su último pago fue realizado el día " + lastPaymentDate + ".";

            var entryInfo = "Su entrada en " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " no pudo ser registrada exitosamente.";

            var bodyData = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "UserName", fullName },
                    { "message", details },
                    { "entryDateInfo", entryInfo },
                    { "lastPaymentDateInfo" , lastPaymentDate }
                };

            _sendEmail.SendEmail(bodyData,
                                 "AssistanceUserNotEntryTemplate",
                                 "Notificación de entrada no procesada" + fullName,
                                 new System.Collections.Generic.List<string>() { email }
                                );

        }

        public void ProcessAssistanceNotification(int userId)
        {
            var users = from m in _context.User
                        select m;

            var user = users.First(u => u.UserId == userId);

            if (user != null)
            {
                var bodyData = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "UserName", user.FullName },
                    { "Title", "Disfrute de la sesión!" },
                    { "message", "Estamos a sus órdenes." }
                };

                _sendEmail.SendEmail(bodyData,
                                     "AssistanceTemplate",
                                     "Notificación de asistencia" + user.FullName,
                                     new System.Collections.Generic.List<string>() { user.Email }
                                    );
            }
        }

        public void ProcessDelete(DateTime assistanceDate, int userId)
        {
            var users = from m in _context.User
                        select m;

            var user = users.First(u => u.UserId == userId);

            if (user != null)
            {
                var bodyData = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "UserName", user.FullName },
                    { "assistanceDate", assistanceDate.ToString("dd/MM/yyyy HH:mm") }
                };

                _sendEmail.SendEmail(bodyData,
                                     "AssistanceDeleteTemplate",
                                     "Notificación de asistencia eliminada" + user.FullName,
                                     new System.Collections.Generic.List<string>() { user.Email }
                                    );
            }
        }

        public void ProcessWelcomeNotification(int userId)
        {
            var users = from m in _context.User
                        select m;

            var user = users.First(u => u.UserId == userId);

            if (user != null)
            {
                _sendEmail.SendEmail(new System.Collections.Generic.Dictionary<string, string>(),
                                     "AssistanceTemplate",
                                     "Notificación de asistencia" + user.FullName,
                                     new System.Collections.Generic.List<string>() { user.Email }
                                    );
            }
        }

    }
}
