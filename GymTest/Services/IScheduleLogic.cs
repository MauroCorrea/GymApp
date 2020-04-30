using System;
namespace GymTest.Services
{
    public interface IScheduleLogic
    {
        bool RegisterUser(int UserId, int ScheduleId);
        int GetSchedulePlaces(int ScheduleId);
    }
}
