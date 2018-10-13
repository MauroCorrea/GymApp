using System;
using System.Linq;
using GymTest.Data;
using GymTest.Models;
using System.Net;
using System.Net.Mail;

namespace GymTest.Services
{
    public class SendEmailImpl : ISendEmail
    {

        private readonly GymTestContext _context;

        public SendEmailImpl(GymTestContext context)
        {
            _context = context;
        }

        public void SendEmail(string mailTo)
        {
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("gymappuyms@gmail.com");
            correo.To.Add(mailTo);
            correo.Subject = "Aviso GymTest";
            correo.Body = "Funciona el envio de mail";
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 25;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = true;
            string sCuentaCorreo = "gymappuyms@gmail.com";
            string pwd = "%TGB6yhn^YHN5tgb";
            smtp.Credentials = new NetworkCredential(sCuentaCorreo, pwd);

            smtp.Send(correo);
        }
    }
}
