using System;
using GymTest.Data;
using GymTest.Models;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Mime;
using Microsoft.Extensions.Logging;

namespace GymTest.Services
{
    public class SendEmailImpl : ISendEmail
    {
        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        private readonly GymTestContext _context;

        private IHostingEnvironment _env;

        private readonly ILogger<ISendEmail> _logger;

        public SendEmailImpl(GymTestContext context, IHostingEnvironment env, IOptionsSnapshot<AppSettings> app, ILogger<ISendEmail> logger)
        {
            _logger = logger;
            _appSettings = app;
            _context = context;
            _env = env;
        }

        private string CreateEmailBody(Dictionary<string, string> bodyData, string templateName)
        {
            try
            {
                string body = string.Empty;
                //using streamreader for reading my htmltemplate  

                var pathToFile = _env.WebRootPath + _appSettings.Value.TemplateEmailPath + templateName + ".html";

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

                body = body.Replace("{client}", _appSettings.Value.Client);

                return body;
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error Sending email. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error Sending email. Detail: " + ex.InnerException.Message);
            }
            return string.Empty;
        }

        public void SendEmailRegister(Dictionary<string, string> bodyData, string templateName, string subject, List<string> receipts)
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
                if (!string.IsNullOrEmpty(correo.Body))
                {
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
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error Sending email. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error Sending email. Detail: " + ex.InnerException.Message);
            }
        }

        public void SendEmail(Dictionary<string, string> bodyData, string templateName, string subject, List<string> receipts, List<string> filePathAttachment = null)
        {
            try
            {
                MailMessage correo = new MailMessage
                {
                    From = new MailAddress(_appSettings.Value.EmailConfiguration_Username)
                };


                if (filePathAttachment != null)
                {
                    foreach (string item in filePathAttachment)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            Attachment attachment = new Attachment(item, MediaTypeNames.Application.Octet);
                            correo.Attachments.Add(attachment);
                        }
                    }
                }

                foreach (var receipt in receipts)
                {
                    correo.To.Add(receipt);
                }

                correo.Subject = subject;
                correo.Body = CreateEmailBody(bodyData, templateName);
                if (!string.IsNullOrEmpty(correo.Body))
                {
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
                    string blindCopy = _appSettings.Value.EmailConfiguration_BlindCopy;

                    if (!string.IsNullOrEmpty(blindCopy))
                        correo.Bcc.Add(_appSettings.Value.EmailConfiguration_BlindCopy);

                    smtp.Credentials = new NetworkCredential(sCuentaCorreo, pwd);

                    smtp.Send(correo);
                }
            }
            catch (Exception ex)
            {
                var messageError = ex.Message;
                _logger.LogError("Error Sending email. Detail: " + messageError);
                if (ex.InnerException != null)
                    _logger.LogError("Error Sending email. Detail: " + ex.InnerException.Message);
            }
        }
    }
}
