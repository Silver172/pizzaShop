namespace PizzaShop.DataAccess.ViewModels;

public class OrderAppKOTContentViewModel
{
    public int OrderId { get; set; }
    public List<string> Tables { get; set; } = null!;
    public string Section { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? OrderDuration { get; set; }
    public string? Notes {get;set;}
    public List<OrderItemViewModel>? OrderItems {get;set;}
}
