
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

        public async Task<IActionResult> test()
        {
         
            ViewBag.ErrorMessage = "You do noit have the right to enter this page";
            return StatusCode(401);
        }
        


        //public IActionResult Privacy()
        //{
        //    return View();
        //}


    }
}
