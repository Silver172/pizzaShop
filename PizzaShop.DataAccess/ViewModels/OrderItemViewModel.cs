namespace PizzaShop.DataAccess.ViewModels;

public class OrderItemViewModel
{
    public int Id { get; set; }

    public int? Menuitemid { get; set; }

    public int? Orderid { get; set; }

    public string? Comment { get; set; }

    public string? Name { get; set; } = null!;

    public short Quantity { get; set; }

    public decimal? Rate { get; set; }

    public decimal? Amount { get; set; }
    public decimal? TotalAmount {get;set;}

    public decimal TotalModifierAmount { get; set; }

    public string? Instruction { get; set; }

    public decimal? Tax { get; set; }

    public List<OrderItemModifierViewModel>? Modifiers { get; set; }

}
