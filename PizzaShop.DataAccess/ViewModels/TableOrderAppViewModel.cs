namespace PizzaShop.DataAccess.ViewModels;

public class TableOrderAppViewModel
{
    public int Id { get; set; }

    public int? Sectionid { get; set; }

    public string Name { get; set; } = null!;

    public short Capacity { get; set; }

    public string Status { get; set; } = null!;

    public string? OrderStatus { get; set; }

    public int? OrderId { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? TimeDuration { get; set; }
}
