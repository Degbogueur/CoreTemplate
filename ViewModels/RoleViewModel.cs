using CoreTemplate.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CoreTemplate.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<ApplicationUser> Users { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}