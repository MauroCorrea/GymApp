using GymTest.Models;

namespace GymTest.Services
{
    public interface IAssistanceLogic
    {
        AssistanceInformation ProcessAssistance(string userToken);
        void ProcessAssistanceNotification(int userId);
        void ProcessWelcomeNotification(int userId);
    }
}
