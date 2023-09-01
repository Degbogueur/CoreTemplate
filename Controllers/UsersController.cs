using CoreTemplate.Models.Authentication;
using CoreTemplate.Models.Commons;
using CoreTemplate.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CoreTemplate.Controllers
{
    public class UsersController : BaseController
    {
        public IActionResult Index()
        {
            var users = _userManager.Users
                .Where(u => !u.IsDeleted)
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

        public IActionResult Create()
        {
            var model = new CreateUserViewModel
            {
                Roles = new SelectList(_roleManager.Roles.ToList(), "Id", "Name")
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel viewModel)
        {
            StatusMessage statusMessage;

            var user = new ApplicationUser
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                EmailConfirmed = viewModel.EmailConfirmed
            };

            var result = await _userManager.CreateAsync(user, viewModel.Password);

            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(viewModel.RoleId);
                await _userManager.AddToRoleAsync(user, role.Name);

                statusMessage = new SuccessMessage("Nouvel utilisateur ajouté avec succès.");

                TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                statusMessage = new ErrorMessage($"Erreur: {string.Join(" ", result.Errors.Select(e => e.Description))}");
            }

            TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                var statusMessage = new ErrorMessage("Utilisateur introuvable.");

                TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            StatusMessage statusMessage;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    statusMessage = new ErrorMessage("Utilisateur introuvable.");

                    TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
                    return RedirectToAction(nameof(Index));
                }

                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    statusMessage = new SuccessMessage("Infos utilisateurs mises à jour avec succès.");

                    TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    statusMessage = new ErrorMessage($"Erreur: {string.Join(" ", result.Errors.Select(e => e.Description))}");
                    TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
                }
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            StatusMessage statusMessage;

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                statusMessage = new ErrorMessage("Utilisateur introuvable.");
            }
            else
            {
                user.IsDeleted = true;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    statusMessage = new SuccessMessage("Utilisateur supprimé avec succès.");
                }
                else
                {
                    statusMessage = new ErrorMessage($"Erreur: {string.Join(" ", result.Errors.Select(e => e.Description))}");
                }
            }

            TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
            return RedirectToAction(nameof(Index));
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

            statusMessage = new SuccessMessage("Mise à jour des rôles de l'utilisateur effectuée avec succès.");
            
            TempData["StatusMessage"] = JsonConvert.SerializeObject(statusMessage);
            return RedirectToAction(nameof(Index));
        }
    }
}
