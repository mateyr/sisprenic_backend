namespace sisprenic.Entities;

public class Menu
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Route { get; set; }

    public string? Icon { get; set; }

    public string? Section { get; set; }

    public int Order { get; set; }

    public string? RequiredClaim { get; set; }

    public int? ParentMenuId { get; set; }

    public Menu? ParentMenu { get; set; }

    public List<Menu> SubMenus { get; } = new();
}