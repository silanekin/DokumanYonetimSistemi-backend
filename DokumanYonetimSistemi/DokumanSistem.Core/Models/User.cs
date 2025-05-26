using Microsoft.AspNetCore.Identity;

namespace DokumanSistem.Core.Models
{
    public class User : BaseEntity<int>
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
