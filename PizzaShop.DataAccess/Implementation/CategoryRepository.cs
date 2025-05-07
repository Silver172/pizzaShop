using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class CategoryRepository : ICategoryRepository
{
    private readonly PizzashopContext _context;
    private readonly IUserRepository _user;
    private readonly IRoleRepository _role;
    public CategoryRepository(PizzashopContext context, IUserRepository user, IRoleRepository role)
    {
        _context = context;
        _user = user;
        _role = role;
    }

    public async Task<List<Category>> GetAllCategories()
    {
        return await _context.Categories.Where(c => c.Isdeleted == false).OrderBy(c => c.Id).ToListAsync();
    }

    public async Task<int> IsCategoryExist(string name)
    {
        var categories = await _context.Categories.Where(m => m.Isdeleted == false && m.Name.ToLower() == name.ToLower()).CountAsync();
        return categories;
    }

    public async Task<bool> IsEditCategoryExist(string name, int? id)
    {
        var categories = await _context.Categories.Where(m => m.Isdeleted == false && m.Name.ToLower() == name.ToLower()).ToListAsync();
        foreach (var c in categories)
        {
            if (c.Id != id)
                return true;
        }
        return false;
    }
    public async Task<bool> AddCategory(AddEditCategoryViewModel model, string userId)
    {
        var newCategory = new Category
        {
            Name = model.Name,
            Description = string.IsNullOrEmpty(model.Description) ? "" : model.Description,
            Createdby = userId,
            Isdeleted = false,
        };

        await _context.Categories.AddAsync(newCategory);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> EditCategory(AddEditCategoryViewModel model, string userId)
    {   
        var editCategory = _context.Categories.FirstOrDefault(c => c.Id == model.Id);
        editCategory.Name = model.Name;
        editCategory.Description = model.Description;
        editCategory.Updatedby = userId;
        editCategory.Updateddate = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategory(int id,string userId)
    {
        var deleteCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (deleteCategory == null)
            return false;
        deleteCategory.Isdeleted = true;
        deleteCategory.Updateddate = DateTime.Now;
        deleteCategory.Updatedby = userId;
        await _context.SaveChangesAsync();
        return true;
    }
}
