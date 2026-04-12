
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


        public IActionResult Privacy()
        {
            return View();
        }

        // testing different errors method 
        public async Task<IActionResult> test()
        {
         
            ViewBag.ErrorMessage = "Message will not get passed. etеrning specific status code.";
            return StatusCode(401);
        }


      


    }
}
