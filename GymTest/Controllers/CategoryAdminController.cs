using Microsoft.AspNetCore.Mvc;

namespace GymTest.Controllers
{
    public class CategoryAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
       
    }
}
