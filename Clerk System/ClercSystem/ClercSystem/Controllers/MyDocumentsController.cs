using Microsoft.AspNetCore.Mvc;

namespace ClercSystem.Controllers
{
    public class MyDocumentsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
