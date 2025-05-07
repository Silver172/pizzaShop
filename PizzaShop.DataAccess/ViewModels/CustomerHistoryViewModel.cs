namespace PizzaShop.DataAccess.ViewModels;

public class CustomerHistoryViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? ComingSince { get; set; }

    public int? Visits { get; set; }

    public decimal? MaxOrder { get; set; }

    public decimal? AverageBill { get; set; }

    public List<CustomerOrderViewModel>? OrderList { get; set; }

}
