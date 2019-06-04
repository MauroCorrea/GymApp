using System.Collections.Generic;

namespace GymTest.Services
{
    public interface ISendEmail
    {
        void SendEmail(Dictionary<string, string> bodyData, string templateName, string subject, List<string> receipts, List<string> filePathAttachment = null);

        void SendEmailRegister(Dictionary<string, string> bodyData, string templateName, string subject, List<string> receipts);

    }
}
