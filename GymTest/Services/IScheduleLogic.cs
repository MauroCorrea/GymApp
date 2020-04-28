﻿using System;
namespace GymTest.Services
{
    public interface IScheduleLogic
    {
        bool RegisterUser(int userId, int scheduleId);
        int GetSchedulePlaces(int scheduleId);
    }
}
