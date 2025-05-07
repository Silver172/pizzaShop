namespace PizzaShop.DataAccess.ViewModels;

public class OrderAppKOTViewModel
{
    public List<CategoriesViewModel> CategoryList {get;set;}
    public List<OrderAppKOTContentViewModel>? ListKotContent {get;set;}

    public PaginationViewModel Pagination {get;set;}
}
