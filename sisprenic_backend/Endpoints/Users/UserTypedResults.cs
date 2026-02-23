using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using sisprenic.Database;
using sisprenic.Entities;

using sisprenic_backend.Dtos.Users;
using sisprenic_backend.Mapping;

namespace sisprenic_backend.Endpoints.Users
{
    public static class UserTypedResults
    {
        public static async Task<IResult> GetCurrentUser(
            ClaimsPrincipal claimsPrincipal,
            SisprenicContext db
        )
        {
            string userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email)!; 

            string[] permissions = claimsPrincipal.Claims
                .Where(claim => claim.Type == "permission")
                .Select(claim => claim.Value)
                .ToArray();

            List<Menu> flatMenu = await db.Menu
                .Where(menu => permissions.Contains(menu.RequiredClaim))
                .OrderBy(menu => menu.Section)
                .ThenBy(menu => menu.Order)
                .AsNoTracking()
                .ToListAsync();

            List<MenuItemDto> menu = BuildMenuTree(flatMenu);

            return TypedResults.Ok(new MeResponseDto(userId, email, menu));
        }

        private static List<MenuItemDto> BuildMenuTree(List<Menu> flatMenu)
        {
            Dictionary<int, MenuItemDto> map = flatMenu
                .Select(m => m.ToMenuItemDto())
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
    }
}
