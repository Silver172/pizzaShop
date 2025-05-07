using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IMenuItemRepository
{
    public Task<List<Menuitem>> GetMenuItemsByCategoryId(int cId, int pageSize, int pageIndex, string? searchString);
    public Task<bool> DeleteMenuItemsByCategoryId(int id,string userId);
    public Task<int> GetItemsCountByCId(int cId,string? searchString);
    public Task<Menuitem> AddItem(AddEditItemViewModel model, string userId);
    public Task<bool> IsItemExist(string name, int catId);
    public Task<bool> IsEditItemExist(string name, int catId,int id);
    public Task<Menuitem> GetMenuItemById(int id);
    public Task EditMenuItem(AddEditItemViewModel model,string userId);
    public Task DeleteMenuItem(int id,string userId);
    public Task<List<Menuitem>> GetMenuItemsByCategoryId(int cId,string searchString);
    public Task<bool> MarkAsFavourite(int itemId);
}
