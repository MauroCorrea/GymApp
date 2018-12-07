using System;
using System.Linq;
using GymTest.Data;
using GymTest.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GymTest.Services
{
    public class AssistanceLogicImpl : IAssistanceLogic
    {

        private readonly GymTestContext _context;

        public IConfiguration _configuration { get; }

        private readonly ISendEmail _sendEmail;

        private readonly IOptions<AppSettings> _appSettings;

        public AssistanceLogicImpl(GymTestContext context, IConfiguration configuration, IOptions<AppSettings> app, ISendEmail sendEmail)
        {
            _appSettings = app;
            _context = context;
            _configuration = configuration;
            _sendEmail = sendEmail;
        }

        public bool ProcessAssistance(string userToken)
        {
            var objectToReturn = false;
            var users = from m in _context.User
                        select m;

            users = users.Where(s => s.Token.ToLower().Equals(userToken.ToLower()));

            if (users.Count() == 1)
            {
                var userid = users.First().UserId;

                var payments = from m in _context.Payment
                               select m;
                payments = payments.Where(p => p.UserId == userid);

                if (payments.Count() > 0)
                {
                    var newestPayment = payments.OrderByDescending(p => p.PaymentDate).First();
                    if (newestPayment.MovementTypeId > 0)
                    {
                        if (newestPayment.LimitUsableDate > DateTime.Now)
                            return objectToReturn;

                        switch (newestPayment.MovementTypeId)
                        {
                            #region Mensual
                            case (int)PaymentTypeEnum.Monthly:
                                var monthsPayed = newestPayment.QuantityMovmentType;

                                var monthsUsed = DateTime.Now.Month - newestPayment.PaymentDate.Month;

                                if (DateTime.Now.Year > newestPayment.PaymentDate.Year)
                                    monthsUsed += 12;

                                if (monthsUsed > monthsPayed)
                                    return objectToReturn;

                                break;
                            #endregion
                            #region Por asistencias
                            case (int)PaymentTypeEnum.ByAssistances:
                                var ass = from a in _context.Assistance select a;

                                ass = ass.Where(a => a.UserId == userid &&
                                                a.AssistanceDate.Date >= newestPayment.PaymentDate.Date);

                                if (ass.Count() >= newestPayment.QuantityMovmentType)
                                    return objectToReturn;
                                break;
                            #endregion

                            default:
                                return objectToReturn;
                        }
                        var lastAsistance = _context.Assistance.
                                                    Where(a => a.UserId == userid).
                                                    OrderByDescending(a => a.AssistanceDate).FirstOrDefault();
                        if (lastAsistance == null ||
                            (DateTime.Now - lastAsistance.AssistanceDate).TotalHours > int.Parse(_appSettings.Value.AssistanceConfiguration_DiffHours))
                        {
                            //Creamos asistencia en caso de que el usuario pueda entrar. Caso contrario, queda a criterio del lugar si pasa o no.
                            Assistance assistance = new Assistance
                            {
                                User = users.First(),
                                AssistanceDate = DateTime.Now
                            };
                            _context.Assistance.Add(assistance);
                            _context.SaveChangesAsync();
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

            return true;
        }

        public void ProcessAssistanceNotification(string fingerprint)
        {
            var users = from m in _context.User
                        select m;

            users = users.Where(s => s.Token.ToLower().Equals(fingerprint.ToLower()));

            if (users.Count() == 1)
            {
                var user = users.First();

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

        public void ProcessWelcomeNotification(string fingerprint)
        {
            var users = from m in _context.User
                        select m;

            users = users.Where(s => s.Token.ToLower().Equals(fingerprint.ToLower()));

            if (users.Count() == 1)
            {
                var user = users.First();

                _sendEmail.SendEmail(new System.Collections.Generic.Dictionary<string, string>(),
                                     "AssistanceTemplate",
                                     "Notificación de asistencia" + user.FullName,
                                     new System.Collections.Generic.List<string>() { user.Email }
                                    );

            }
        }

    }
}
