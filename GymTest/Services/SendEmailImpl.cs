using System;
using GymTest.Data;
using GymTest.Models;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace GymTest.Services
{
    public class SendEmailImpl : ISendEmail
    {
        private readonly IOptions<AppSettings> _appSettings;

        private readonly GymTestContext _context;

        private IHostingEnvironment _env;

        public SendEmailImpl(GymTestContext context, IHostingEnvironment env, IOptions<AppSettings> app)
        {
            _appSettings = app;
            _context = context;
            _env = env;
        }

        private string CreateEmailBody(Dictionary<string, string> bodyData, string templateName)
        {
            string body = string.Empty;
            //using streamreader for reading my htmltemplate  

            var pathToFile = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                                 + templateName + ".html";

            using (StreamReader reader = File.OpenText(pathToFile))
            {
                body = reader.ReadToEnd();
            }

            foreach (var key in bodyData.Keys)
            {
                if (body.Contains("{" + key + "}"))
                {
                    body = body.Replace("{" + key + "}", bodyData.GetValueOrDefault(key)); //replacing the required things
                }
            }

            return body;
        }

        public void SendEmail(Dictionary<string, string> bodyData, string templateName, string subject, List<string> receipts)
        {
            try
            {
                MailMessage correo = new MailMessage
                {
                    From = new MailAddress(_appSettings.Value.EmailConfiguration_Username)
                };
                foreach (var receipt in receipts)
                {
                    correo.To.Add(receipt);
                }
                correo.Subject = subject;
                correo.Body = CreateEmailBody(bodyData, templateName);
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;

                SmtpClient smtp = new SmtpClient
                {
                    Host = _appSettings.Value.EmailConfiguration_Host,
                    Port = int.Parse(_appSettings.Value.EmailConfiguration_Port),
                    EnableSsl = true,
                    UseDefaultCredentials = true
                };
                string sCuentaCorreo = _appSettings.Value.EmailConfiguration_Username;
                string pwd = _appSettings.Value.EmailConfiguration_Password;
                smtp.Credentials = new NetworkCredential(sCuentaCorreo, pwd);

                smtp.Send(correo);
            }
            catch (Exception ex)
            {
                // TODO: log error
                var messageError = ex.Message;

            }
        }
    }
}
