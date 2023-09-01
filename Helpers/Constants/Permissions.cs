using static CoreTemplate.Helpers.Constants.Enumerations;

namespace CoreTemplate.Helpers.Constants
{
    public static class Permissions
    {
        private static readonly Dictionary<string, Func<List<string>>> PermissionGenerators = new Dictionary<string, Func<List<string>>>
        {
            { "Users", Users.Permissions },
            { "Roles", Roles.Permissions }
        };

        public static List<string> GeneratePermissionsList(string module)
        {
            if (PermissionGenerators.TryGetValue(module, out var permissionsGen))
            {
                return permissionsGen();
            }

            return null;
        }

        public static List<string> GenerateAllPermissions()
        {
            var allPermissions = new List<string>();
            var modules = Enum.GetValues(typeof(Modules));

            foreach (var module in modules)
            {
                allPermissions.AddRange(GeneratePermissionsList(module.ToString()));
            }

            return allPermissions;
        }

        public static class Users
        {
            public const string Create = "Users.Permission: Ajouter un nouvel utilisateur";

            public static List<string> Permissions() => new List<string>()
            {
                Create
            };
        }

        public static class Roles
        {
            public const string Create = "Roles.Permission: Ajouter un nouveau rôle";
            public const string Edit = "Roles.Permission: Modifier un rôle";
            public const string Delete = "Roles.Permission: Supprimer un rôle";
            public const string ManagePermissions = "Roles.Permission: Configurer les habilitations d'un rôle";

            public static List<string> Permissions() => new List<string>()
            {
                Create, Edit, Delete, ManagePermissions
            };
        }
    }
}
