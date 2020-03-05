using System;
namespace GymTest.Services
{
    public interface ITimezoneLogic
    {
        DateTime GetCurrentDateTime(DateTime dateTime);
    }
}
