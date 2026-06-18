using System.Security.Claims;

using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;
using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Modules.Users.GetCurrentUser;

public static class GetCurrentUserEndpoint
{
    public static void MapGetCurrentUser(this RouteGroupBuilder group)
    {
        group.MapGet("/me", Handle).RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal claimsPrincipal,
        SisprenicContext db,
        CancellationToken cancellationToken)
    {
        string userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email)!;

        string[] permissions = claimsPrincipal.Claims
            .Where(claim => claim.Type == Permissions.ClaimType)
            .Select(claim => claim.Value)
            .ToArray();

        List<Menu> flatMenu = await db.Menu
            .Where(menu => permissions.Contains(menu.RequiredClaim))
            .OrderBy(menu => menu.SectionOrder)
            .ThenBy(menu => menu.Order)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        List<MenuItemDto> menu = BuildMenuTree(flatMenu);

        return TypedResults.Ok(new MeResponseDto(userId, email, menu));
    }

    private static List<MenuItemDto> BuildMenuTree(List<Menu> flatMenu)
    {
        Dictionary<int, MenuItemDto> map = flatMenu
            .Select(ToMenuItemDto)
            .ToDictionary(m => m.Id);

        List<MenuItemDto> menu = new();

        foreach (var menuItem in flatMenu)
        {
            MenuItemDto dto = map[menuItem.Id];

            if (menuItem.ParentMenuId is null)
            {
                menu.Add(dto);
            }
            else if (map.TryGetValue(menuItem.ParentMenuId.Value, out var parentMenu))
            {
                parentMenu.SubMenus.Add(dto);
            }
        }

        return menu;
    }

    private static MenuItemDto ToMenuItemDto(Menu menu)
    {
        return new MenuItemDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Route = menu.Route,
            Icon = menu.Icon,
            Section = menu.Section
        };
    }
}
