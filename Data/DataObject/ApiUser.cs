using Microsoft.AspNetCore.Identity;

namespace Data.DataObjects
{
    public class ApiUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}