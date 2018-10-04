using System;
namespace GymTest.Services
{
    public interface IAssistanceLogic
    {
        bool ProcessAssistance(string userToken);
    }
}
