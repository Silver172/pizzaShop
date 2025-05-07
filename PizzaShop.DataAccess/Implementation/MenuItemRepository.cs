using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly PizzashopContext _context;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IRoleRepository _role;
    public MenuItemRepository(PizzashopContext context, ICategoryRepository categoryRepository, IRoleRepository role)
    {
        _context = context;
        _categoryRepository = categoryRepository;
        _role = role;
    }

    public async Task<List<Menuitem>> GetMenuItemsByCategoryId(int cId, int pageSize, int pageIndex, string? searchString)
    {
        var query = _context.Menuitems.Where(i => i.Categoryid == cId && i.Isdeleted == false);

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchString.Trim().ToLower()));
        }

        var itemList = await query.OrderBy(u => u.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(i => i.Name)
            .ToListAsync();
        return itemList;
    }

    public async Task<int> GetItemsCountByCId(int cId,string? searchString)
    {
        var query = _context.Menuitems.Where(i => i.Categoryid == cId && i.Isdeleted == false);

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchString.Trim().ToLower()));
        }

        var items = await query.OrderBy(u => u.Id)
            .OrderBy(i => i.Name).ThenBy(i => i.Id)
            .ToListAsync();
        int count = items.Count();
        return count;
    }

    public async Task<bool> DeleteMenuItemsByCategoryId(int id,string userId)
    {
        var items = await _context.Menuitems.Where(m => m.Categoryid == id).ToListAsync();
        if (items.Count > 0)
        {
            foreach (var item in items)
            {
                item.Isdeleted = true;
                item.Updatedby = userId;
                item.Updateddate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }
        return true;
    }

    public async Task<Menuitem> AddItem(AddEditItemViewModel model, string userId)
    {
        var menuItem = new Menuitem
        {
            Name = model.Name,
            Categoryid = model.CategoryId,
            Itemtype = model.Itemtype,
            Rate = model.Rate,
            Quantity = model.Quantity,
            Isavailable = model.Isavailable,
            Isdeleted = false,
            Unitid = model.Unitid,
            Isdefaulttax = model.Isdefaulttax,
            Taxpercentage = model.Taxpercentage,
            Shortcode = model.Shortcode,
            Description = model.Description,
            Itemimage = model.ImagePath,
            Createddate = DateTime.Now,
            Createdby = userId
        };
        await _context.Menuitems.AddAsync(menuItem);
        await _context.SaveChangesAsync();
        return menuItem;
    }

    public async Task<bool> IsItemExist(string name, int catId)
    {
        var item = await _context.Menuitems.Where(i => i.Name.ToLower() == name.ToLower() && i.Categoryid == catId && i.Isdeleted == false).ToListAsync();
        var count = item.Count();
        if (count > 0)
            return true;
        return false;
    }

    public async Task<bool> IsEditItemExist(string name, int catId,int id)
    {
        var items = await _context.Menuitems.Where(i => i.Name.ToLower() == name.ToLower() && i.Categoryid == catId && i.Isdeleted == false).ToListAsync();
        foreach (var m in items)
        {
            if(m.Id != id)
                return true;
        }
        return false;
    }

    public async Task<Menuitem> GetMenuItemById(int id)
    {
        var item = await _context.Menuitems.FirstOrDefaultAsync(i => i.Id == id);
        return item;
    }

    public async Task EditMenuItem(AddEditItemViewModel model,string userId)
    {
        var menuItem = await _context.Menuitems.FirstOrDefaultAsync(i => i.Id == model.id);
        
        menuItem.Categoryid = model.CategoryId;
        menuItem.Name = model.Name;
        menuItem.Itemtype = model.Itemtype;
        menuItem.Rate = model.Rate;
        menuItem.Quantity = model.Quantity;
        menuItem.Isavailable = model.Isavailable;
        menuItem.Unitid = model.Unitid;
        menuItem.Isdefaulttax = model.Isdefaulttax;
        menuItem.Taxpercentage = model.Taxpercentage;
        menuItem.Shortcode = model.Shortcode;
        menuItem.Description = model.Description;
        menuItem.Itemimage = model.ImagePath;
        menuItem.Updatedby = userId;
        menuItem.Updateddate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMenuItem(int id,string userId)
    {
        var menuItem = await _context.Menuitems.FirstOrDefaultAsync(i => i.Id == id);
        menuItem.Isdeleted = true;
        menuItem.Updatedby = userId;
        menuItem.Updateddate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Menuitem>> GetMenuItemsByCategoryId(int cId,string searchString)
    {
        
        var query = _context.Menuitems.Where(i => (cId == 0 || i.Categoryid == cId || cId == -1) && i.Isdeleted == false);
        if(!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchString.Trim().ToLower()));
        }
        if (cId == -1)
        {
            query = query.Where(i => i.Isfavourite == true);
        }
        var itemList = await query.OrderBy(u => u.Id)
            .OrderBy(i => i.Name)
            .ToListAsync();

        return itemList;
    }

    public async Task<bool> MarkAsFavourite(int itemId)
    {
        Menuitem item = await _context.Menuitems.FirstOrDefaultAsync(i => i.Id == itemId && i.Isdeleted == false);
        if (item != null)
        {
            item.Isfavourite = !item.Isfavourite;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
