namespace PizzaShop.DataAccess.ViewModels;

public class CustomerOrderViewModel
{
    public int Id { get; set; }
    public string OrderDate {get;set;}
    public string? OrderType { get; set; }
    public string? PaymentStatus { get; set; }
    public int Items { get; set; }
    public decimal? Amount { get; set; }
}
