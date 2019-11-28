using System;
using GymTest.Models;

namespace GymTest.Services
{
    public interface IAssistanceLogic
    {
        AssistanceInformation ProcessAssistance(string userToken, DateTime? assistanceDzte = null);
        void ProcessAssistanceNotification(int userId, int remainingAssistants);
        void ProcessWelcomeNotification(int userId);
        void ProcessDelete(DateTime assistanceDate, int userId);
    }
}
