using Microsoft.AspNetCore.Identity;

namespace dokumansistem.Models
{
    public class User:IdentityUser
    {
        public int userId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
    }
}
