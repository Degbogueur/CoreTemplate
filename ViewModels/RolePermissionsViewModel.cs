namespace CoreTemplate.ViewModels
{
    public class RolePermissionsViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public IList<PermissionsGroupViewModel> PermissionsGroups { get; set; }
    }

    public class PermissionsGroupViewModel
    {
        public string GroupName { get; set; }
        public IList<CheckBoxViewModel> Permissions { get; set; }
    }
}
