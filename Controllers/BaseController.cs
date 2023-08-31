using CoreTemplate.Data;
using CoreTemplate.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreTemplate.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationDbContext Context;
        private UserManager<ApplicationUser> UserManager;
        private RoleManager<IdentityRole> RoleManager;

        protected ApplicationDbContext _context => Context ?? HttpContext.RequestServices.GetService<ApplicationDbContext>();
        protected UserManager<ApplicationUser> _userManager => UserManager ?? HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
        protected RoleManager<IdentityRole> _roleManager => RoleManager ?? HttpContext.RequestServices.GetService<RoleManager<IdentityRole>>();
    }
}
