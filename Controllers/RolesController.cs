using CoreTemplate.Helpers.Constants;
using CoreTemplate.Models.Commons;
using CoreTemplate.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CoreTemplate.Controllers
{
    public class RolesController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            var roles = new List<RoleViewModel>();

            foreach (var role in _roleManager.Roles.ToList())
            {
                roles.Add(new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    Users = await _userManager.GetUsersInRoleAsync(role.Name),
                    Claims = await _roleManager.GetClaimsAsync(role)
                });
            }

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

        public async Task<IActionResult> ManagePermissions(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                var errorMessage = new ErrorMessage("Rôle introuvable.");

                TempData["StatusMessage"] = JsonConvert.SerializeObject(errorMessage);
                return RedirectToAction(nameof(Index));
            }

            var allClaims = Permissions.GenerateAllPermissions();
            var roleClaims = _roleManager.GetClaimsAsync(role).Result.Select(c => c.Value).ToList();

            var groupedClaims = allClaims.Select(claim => new CheckBoxViewModel
            {
                Value = claim,
                IsSelected = roleClaims.Contains(claim)
            }).GroupBy(claim => claim.Value.Split('.')[0].Trim())
            .Select(group => new PermissionsGroupViewModel
            {
                GroupName = group.Key,
                Permissions = group.ToList()
            });

            var rolePermissions = new RolePermissionsViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                PermissionsGroups = groupedClaims.ToList()                
            };

            return View(rolePermissions);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePermissions(RolePermissionsViewModel rolePermissions)
        {
            StatusMessage statusMessage;
            var role = await _roleManager.FindByIdAsync(rolePermissions.RoleId);

            if (role == null)
            {
                statusMessage = new ErrorMessage("Rôle introuvable.");

                TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
                return RedirectToAction(nameof(ManagePermissions), new { roleId = rolePermissions.RoleId });
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            
            foreach (var group in rolePermissions.PermissionsGroups)
            {
                foreach (var claim in group.Permissions)
                {
                    if (claim.IsSelected)
                    {
                        await _roleManager.AddClaimAsync(role, new Claim("Permission", claim.Value));
                    }
                }
            }

            statusMessage = new SuccessMessage("Mise à jour des habilitations liées au rôle effectuée avec succès.");

            TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
            return RedirectToAction(nameof(Index));
        }
    }
}