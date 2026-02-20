namespace sisprenic_backend.Dtos.Users;

public record MeResponseDto(string user_name, string email, List<MenuItemDto> menu);

public class MenuItemDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Route { get; set; }
    public string? Icon { get; set; }
    public string? Section { get; set; }
    public int? ParentId { get; set; }
    public int Order { get; set; }
    public List<MenuItemDto>? SubMenus { get; set; }
}
