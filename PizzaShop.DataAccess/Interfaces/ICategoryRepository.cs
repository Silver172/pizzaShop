using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface ICategoryRepository
{
    public Task<List<Category>> GetAllCategories();
    public Task<bool> AddCategory(AddEditCategoryViewModel model, string email);
    public Task<Category?> GetCategoryById(int id);
    public Task<bool> EditCategory(AddEditCategoryViewModel model, string email);
    public Task<bool> DeleteCategory(int id,string userId);
    public Task<int> IsCategoryExist(string name);
    public Task<bool> IsEditCategoryExist(string name, int? id);
}
