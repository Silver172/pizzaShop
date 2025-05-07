using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.ViewModels;

public class ItemTabViewModel
{
    public List<CategoriesViewModel> ListCategories { get; set; }
    public List<ItemsViewModel>? ListItems { get; set; }

    public AddEditItemViewModel? AddEditItem { get; set; }
    public CategoriesViewModel Category { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public string? SearchString { get; set; }
    public int TotalPage { get; set; }
    public int TotalRecord { get; set; }
}
