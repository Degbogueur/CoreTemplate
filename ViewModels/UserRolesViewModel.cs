namespace CoreTemplate.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public IList<CheckBoxViewModel> Roles { get; set; }
    }
}
