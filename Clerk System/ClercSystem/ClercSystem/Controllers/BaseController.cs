using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClercSystem.Controllers
{
    public class BaseController : Controller
    {
        // Method to get the user id from the claims
        protected string GetUserId()
           => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public Guid GetUserIdAsGuid()
        {
            string userId = GetUserId();
            return Guid.TryParse(userId, out Guid userGuid) ? userGuid : Guid.Empty;
        }

        internal bool CheckIfGuidIsValid(string id)
        {
            bool isValid = false;
            if (!string.IsNullOrWhiteSpace(id))
            {
                isValid = Guid.TryParse(id, out Guid result);
            }
             
            return isValid;
        }
    }
}
