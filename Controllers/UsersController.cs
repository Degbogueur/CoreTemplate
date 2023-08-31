using CoreTemplate.Models.Commons;
using CoreTemplate.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CoreTemplate.Controllers
{
    public class UsersController : BaseController
    {
        public IActionResult Index()
        {
            var users = _userManager.Users
                .Select(user => new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = _userManager.GetRolesAsync(user).Result
                }).ToList();

            return View(users);
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            StatusMessage statusMessage;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                statusMessage = new ErrorMessage("Utilisateur introuvable.");

                TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
                return RedirectToAction(nameof(Index));
            }

            var roles = _roleManager.Roles.ToList();
            var userRoles = new UserRolesViewModel
            {
                UserId = user.Id,
                UserFullName = user.FullName,
                Roles = roles.Select(role => new CheckBoxViewModel
                {
                    Value = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };

            return View(userRoles);
        }

        public async Task<IActionResult> UpdateRoles(UserRolesViewModel userRoles)
        {
            StatusMessage statusMessage;
            var user = await _userManager.FindByIdAsync(userRoles.UserId);

            if (user == null)
            {
                statusMessage = new ErrorMessage("Utilisateur introuvable.");

                TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
                return RedirectToAction(nameof(ManageRoles), new { userId = userRoles.UserId });
            }

            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRolesAsync(user, userRoles.Roles.Where(r => r.IsSelected).Select(r => r.Value));

            statusMessage = new SuccessMessage("Mise à jour des rôles de l'utilisateur effectuée avec succès");
            
            TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
            return RedirectToAction(nameof(Index));
        }
    }
}
