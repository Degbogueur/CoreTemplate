using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoreTemplate.ViewModels
{
    public class CreateUserViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Nom")]
        public string? LastName { get; set; }

        [Display(Name = "Prénom(s)")]
        public string? FirstName { get; set; }

        public string? Email { get; set; }

        [Display(Name = "Compte actif")]
        public bool EmailConfirmed { get; set; } = true;

        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmez le mot de passe")]
        [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
        public string? PasswordConfirm { get; set; }

        //public string? PhoneNumber { get; set; }

        [Display(Name = "Nom d'utilisateur")]
        public string? UserName { get; set; }

        [Display(Name = "Rôle")]
        public string? RoleId { get; set; }

        public SelectList Roles { get; set; }
    }
}
