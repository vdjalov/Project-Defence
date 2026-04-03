using Microsoft.AspNetCore.Mvc;

namespace ClercSystem.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
