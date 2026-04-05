
using Microsoft.AspNetCore.Mvc;


namespace ClercSystem.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }




        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
