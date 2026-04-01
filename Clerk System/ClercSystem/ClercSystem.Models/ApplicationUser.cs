using Microsoft.AspNet.Identity.EntityFramework;

namespace ClercSystem.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        
    }
}