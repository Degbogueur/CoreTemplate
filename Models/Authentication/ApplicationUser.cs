using Microsoft.AspNetCore.Identity;

namespace CoreTemplate.Models.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public byte[]? Photo { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}
