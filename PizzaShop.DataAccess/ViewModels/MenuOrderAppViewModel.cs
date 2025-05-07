namespace PizzaShop.DataAccess.ViewModels;

public class MenuOrderAppViewModel
{
  public List<CategoriesViewModel>? CategoriesList { get; set; }
  public List<ItemsViewModel>? ItemsList { get; set; }
  public OrderDetailViewModel? OrderDetails { get; set; }
}
