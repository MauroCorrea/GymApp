using System;
using System.Linq;
using GymTest.Data;

namespace GymTest.Services
{
    public class AssistanceLogicImpl: IAssistanceLogic
    {

        private readonly GymTestContext _context;

        public AssistanceLogicImpl(GymTestContext context)
        {
            _context = context;
        }

        public bool ProcessAssistance(string userToken)
        {
            var objectToReturn = false;
            var users = from m in _context.User
                        select m;

            users = users.Where(s => s.Email.ToLower().Equals(userToken.ToLower()));

            if (users.Count() == 1)
            {
                var userid = users.First().UserId;

                var payments = from m in _context.Payment
                               select m;
                payments = payments.Where(p => p.UserId == userid);
                if(payments.Count()>0){
                    var lastPayment = payments.First();
                    var lastPaymentMode = 1;
                    switch(lastPaymentMode)
                    {
                        case 1:
                            break;
                        default:
                            return objectToReturn;
                    }

                    //var ass = from a in _context.Assistance select a;
                    //ass = ass.Where(a => a.UserId.Equals(users.FirstOrDefault().UserId));

                    //Assistance assistance = new Assistance();
                    //assistance.User = users.FirstOrDefault();
                    //assistance.AssistanceDate = DateTime.Now;
                    //_context.Assistance.Add(assistance);
                    //_context.SaveChangesAsync();
                }
                else{
                    return objectToReturn;
                }

            }
            else{
                //error: solo 1 usuario deber identificado por token
                return objectToReturn;
            }
            return true;
        }
    }
}
