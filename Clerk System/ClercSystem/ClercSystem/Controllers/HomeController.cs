
using Microsoft.AspNetCore.Mvc;


namespace ClercSystem.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Document");
            }

            return View();
        }


        //public IActionResult Privacy()
        //{
        //    return View();
        //}

       
    }
}
