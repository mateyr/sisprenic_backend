using Sisprenic.Api.Authorization;
using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Database;

public static class MenuSeed
{
    public static readonly Menu[] Menu =
    [
        new Menu
        {
            Name = "Dashboard",
            Route = "/dashboard",
            Icon = "IconLayoutDashboard",
            Order = 1,
            Section = "Operación",
            SectionOrder = 1,
            RequiredClaim = Permissions.Dashboard.Read,
        },
        new Menu
        {
            Name = "Clientes",
            Route = "/clients",
            Icon = "IconUsersGroup",
            Order = 2,
            Section = "Operación",
            SectionOrder = 1,
            RequiredClaim = Permissions.Clients.Read,
        },
        new Menu
        {
            Name = "Préstamos",
            Route = "/loans",
            Icon = "IconBuildingBank",
            Order = 3,
            Section = "Operación",
            SectionOrder = 1,
            RequiredClaim = Permissions.Loans.Read,
        },
        new Menu
        {
            Name = "Pagos",
            Route = "/payments",
            Icon = "IconCreditCardPay",
            Order = 4,
            Section = "Operación",
            SectionOrder = 1,
            RequiredClaim = Permissions.Payments.Read,
        },
        new Menu
        {
            Name = "Reportes",
            Route = "/reports",
            Icon = "IconReportAnalytics",
            Order = 5,
            Section = "Operación",
            SectionOrder = 1,
            RequiredClaim = Permissions.Reports.Read,
        },
        new Menu
        {
            Name = "Usuarios",
            Route = "/users",
            Icon = "IconUser",
            Order = 1,
            Section = "Seguridad",
            SectionOrder = 2,
            RequiredClaim = Permissions.Users.Read,
        },
        new Menu
        {
            Name = "Roles",
            Route = "/roles",
            Icon = "IconShield",
            Order = 2,
            Section = "Seguridad",
            SectionOrder = 2,
            RequiredClaim = Permissions.Roles.Read,
        },
        new Menu
        {
            Name = "Configuración",
            Route = "/settings",
            Icon = "settings",
            Order = 1,
            Section = "Configuración",
            SectionOrder = 3,
            RequiredClaim = Permissions.Settings.Read,
        },
    ];
}
