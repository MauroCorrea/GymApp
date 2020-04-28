using System;
using GymTest.Data;
using GymTest.Models;
using Microsoft.Extensions.Logging;

namespace GymTest.Services
{
    public class ScheculeLogicImpl : IScheduleLogic
    {

        private readonly ISendEmail _sendEmail;

        private readonly GymTestContext _context;

        private readonly ILogger<IPaymentLogic> _logger;

        public ScheculeLogicImpl(GymTestContext context, ISendEmail sendEmail, ILogger<IPaymentLogic> logger)
        {
            _logger = logger;
            _context = context;
            _sendEmail = sendEmail;
        }


        public int GetSchedulePlaces(int ScheduleId)
        {
            //TODO: acá debemos de obtener el calendario y si es correcto devolver la cantidad de cupos
            return 0;
        }

        public bool RegisterUser(int UserId, int ScheduleId)
        {
            //TODO: acá debemos de obtener el usuario y si el id del calendario es correcto y tiene cupos ->
            // 1.agregarlo a los participantes
            // 2. devolver true
            // caso contrario devolver false
            return false;
        }
    }
}
