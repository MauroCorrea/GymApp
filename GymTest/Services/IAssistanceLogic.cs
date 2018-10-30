using System;
namespace GymTest.Services
{
    public interface IAssistanceLogic
    {
        bool ProcessAssistance(string userToken);
        void ProcessAssistanceNotification(string fingerprint);
        void ProcessWelcomeNotification(string fingerprint);
    }
}
