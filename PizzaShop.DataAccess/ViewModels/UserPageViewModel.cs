using PizzaShop.ViewModels;

namespace PizzaShop.DataAccess.ViewModels;

public class UserPageViewModel
{
  public List<UsersViewModel> UserList {get;set;} = null!;
  public PaginationViewModel? Pagination {get;set;}
}
