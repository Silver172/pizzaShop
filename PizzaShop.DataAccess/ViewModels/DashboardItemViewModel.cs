namespace PizzaShop.DataAccess.ViewModels;

public class DashboardItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ItemImage { get; set; }

    public int OrderCount { get; set; }
}
