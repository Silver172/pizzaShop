namespace PizzaShop.DataAccess.ViewModels;

public class OrderItemModifierViewModel
{
    public int Id { get; set; }

    public int? ModifierId { get; set; }

    public int? ModifierGroupId { get; set; }

    public int? OrderItemid { get; set; }

    public string? Name { get; set; }

    public short? Quantity { get; set; }

    public decimal? Rate { get; set; }

    public decimal? TotalAmount { get; set; }
}
