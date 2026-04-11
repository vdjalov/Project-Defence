using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClercSystem.Controllers
{
    public class ErrorController : BaseController
    {
        
    //  Handles 500 (unhandled exceptions)
        [Route("Error/500")]
        public IActionResult Error500()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (exceptionFeature != null)
            {
                var exception = exceptionFeature.Error;

             
                Console.WriteLine(exception.Message);
            }

            Response.StatusCode = 500;
            return View("500");
        }

        //  Handles status codes like 404
        [Route("Error/{statusCode}")]
        public IActionResult HandleStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    Response.StatusCode = 404;
                    return View("404");

                case 403:
                    Response.StatusCode = 403;
                    return View("403");

                case 401:
                    Response.StatusCode = 401;
                    return View("401");

                default:
                    return View("Error"); // generic fallback
            }
        }
        
    }
}
