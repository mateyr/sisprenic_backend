using System.Reflection;

namespace Sisprenic.Api.Authorization;

public static class Permissions
{
    public const string ClaimType = "permission";

    public static readonly IReadOnlySet<string> All = typeof(Permissions)
        .GetNestedTypes(BindingFlags.Public)
        .SelectMany(nested => nested.GetFields(BindingFlags.Public | BindingFlags.Static))
        .Where(field => field.IsLiteral && field.FieldType == typeof(string))
        .Select(field => (string)field.GetRawConstantValue()!)
        .ToHashSet();

    public static class Dashboard
    {
        public const string Read = "dashboard:read";
    }

    public static class Clients
    {
        public const string Read = "clients:read";
        public const string Create = "clients:create";
        public const string Update = "clients:update";
        public const string Delete = "clients:delete";
    }

    public static class Loans
    {
        public const string Read = "loans:read";
        public const string Create = "loans:create";
        public const string Update = "loans:update";
        public const string Delete = "loans:delete";
    }

    public static class Payments
    {
        public const string Read = "payments:read";
        public const string Create = "payments:create";
        public const string Delete = "payments:delete";
    }

    public static class Reports
    {
        public const string Read = "reports:read";
    }

    public static class Users
    {
        public const string Read = "users:read";
        public const string Create = "users:create";
    }

    public static class Roles
    {
        public const string Read = "roles:read";
    }

    public static class Settings
    {
        public const string Read = "settings:read";
    }
}
