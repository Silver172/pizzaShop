namespace PizzaShop.DataAccess.ViewModels;

public class OrdersPageViewModel
{
    public List<OrdersViewModel> OrdersList { get; set; } = null!;
    public PaginationViewModel Pagination { get; set; }
}
