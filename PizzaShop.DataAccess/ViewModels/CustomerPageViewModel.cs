namespace PizzaShop.DataAccess.ViewModels;

public class CustomerPageViewModel
{
    public List<CustomerViewModel> CustomerList {get;set;}
  public PaginationViewModel Pagination {get;set;}
}
