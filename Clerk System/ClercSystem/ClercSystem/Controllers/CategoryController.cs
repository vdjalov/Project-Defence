using Microsoft.AspNetCore.Mvc;

namespace ClercSystem.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
