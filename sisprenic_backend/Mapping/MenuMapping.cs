using sisprenic.Entities;

using sisprenic_backend.Dtos.Users;

namespace sisprenic_backend.Mapping;

public static class MenuMapping
{
    public static MenuItemDto ToMenuItemDto(this Menu menu)
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
