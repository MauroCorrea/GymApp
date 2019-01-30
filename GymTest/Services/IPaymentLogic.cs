using GymTest.Models;

namespace GymTest.Services
{
    public interface IPaymentLogic
    {
        bool ProcessPayment(Payment payment, string UserName, string userEmail);
    }
}
