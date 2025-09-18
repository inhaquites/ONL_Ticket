namespace Lenovo.NAT.Infrastructure
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
        {
            $"Permissions.{module}.Create",
            $"Permissions.{module}.View",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Delete",
        };
        }
        public static class AnymarketOrder
        {
            public const string View = "Permissions.AnymarketOrder.View";
            public const string Create = "Permissions.AnymarketOrder.Create";
            public const string Edit = "Permissions.AnymarketOrder.Edit";
            public const string Delete = "Permissions.AnymarketOrder.Delete";
        }

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";

            public const string New = "Permissions.Users.New";
        }
    }
}
