using CoreTemplate.Models.Commons;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CoreTemplate.Controllers
{
    public class RolesController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            roleName = roleName.Trim();
            StatusMessage statusMessage;

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));

                statusMessage = new SuccessMessage("Nouveau rôle créé avec succès.");
            }
            else
            {
                statusMessage = new ErrorMessage("Ce rôle existe déjà.");
            }

            TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            StatusMessage statusMessage;
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) 
            {
                statusMessage = new ErrorMessage("Ce rôle n'existe pas.");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            if (usersInRole.Any())
            {
                statusMessage = new WarningMessage("Ce rôle est déjà attribué à plusieurs utilisateurs.");
            }
            else
            {
                await _roleManager.DeleteAsync(role);

                statusMessage = new SuccessMessage("Rôle supprimé avec succès.");
            }

            TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string roleId, string roleName)
        {
            StatusMessage statusMessage;

            var role = await _roleManager.FindByIdAsync(roleId);
            
            if (role == null)
            {
                statusMessage = new ErrorMessage("Ce rôle n'existe pas.");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                role.Name = roleName;
                await _roleManager.UpdateAsync(role);

                statusMessage = new SuccessMessage("Libellé du rôle mis à jour avec succès.");
            }
            else
            {
                statusMessage = new ErrorMessage("Ce rôle existe déjà.");
            }

            TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
            return RedirectToAction(nameof(Index));
        }
    }
}