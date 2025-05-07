using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class ModifierRepository : IModifierReposotory
{
    private readonly PizzashopContext _context;

    public ModifierRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Modifier>?> GetModifiersByModifierGroupId(int id, PaginationViewModel pagination)
    {
        var query = _context.Modifiers.Where(m => m.Isdeleted == false && m.Modifiergroupid == id);

        if (!string.IsNullOrEmpty(pagination.SearchString))
        {
            var searchString = pagination.SearchString.Trim().ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(searchString));
        }

        var modifiers = await query.OrderBy(i => i.Name)
            .Skip((pagination.PageIndex - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return modifiers;
    }

    public async Task<int> GetModifiersCountByMdfGrpId(int id,string? searchString)
    {
        var query = _context.Modifiers.Where(m => m.Isdeleted == false && m.Modifiergroupid == id);

        if (!string.IsNullOrEmpty(searchString))
        {
            var ss = searchString.Trim().ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(ss));
        }

        var modifiers = await query.OrderBy(i => i.Name)
            .ToListAsync();
        int count = modifiers.Count();
        return count;
    }

    public async Task<List<Modifier>?> GetAllModifiers()
    {
        return await _context.Modifiers.Where(m => m.Isdeleted == false).ToListAsync().ContinueWith(items => items.Result.DistinctBy(i => i.Name).ToList());
    }

    public async Task<List<Modifier>?> GetModifiersByModifierGroup(int? id)
    {
        return await _context.Modifiers.Where(m => m.Modifiergroupid == id && m.Isdeleted == false).ToListAsync();
    }

    public async Task AddExistingModifiers(List<ExistingModifierViewModel> modifiers, int modifierGroupId, string userId)
    {
        foreach (var m in modifiers)
        {
            var existingModifier = await _context.Modifiers.FirstOrDefaultAsync(a => a.Id == m.Id);
            var newModifier = new Modifier
            {
                Name = existingModifier.Name,
                Description = existingModifier.Description,
                Modifiergroupid = modifierGroupId,
                Rate = existingModifier.Rate,
                Quantity = existingModifier.Quantity,
                Unitid = existingModifier.Unitid,
                Createdby = userId,
                Createddate = DateTime.Now,
                Isdeleted = false,
            };

            await _context.Modifiers.AddAsync(newModifier);
        }
        await _context.SaveChangesAsync();
    }

    public async Task AddExistingModifier(ExistingModifierViewModel modifier, int? modifierGroupId, string userId)
    {
        var existingModifier = await _context.Modifiers.FirstOrDefaultAsync(a => a.Id == modifier.Id);

        var newModifier = new Modifier
        {
            Name = existingModifier.Name,
            Description = existingModifier.Description,
            Modifiergroupid = modifierGroupId,
            Rate = existingModifier.Rate,
            Quantity = existingModifier.Quantity,
            Unitid = existingModifier.Unitid,
            Createdby = userId,
            Createddate = DateTime.Now,
            Isdeleted = false,
        };

        await _context.Modifiers.AddAsync(newModifier);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteModifier(int id,string userId)
    {
        var modifier = await _context.Modifiers.FirstOrDefaultAsync(a => a.Id == id);
        modifier.Isdeleted = true;
        modifier.Updateddate = DateTime.Now;
        modifier.Updatedby = userId;
        await _context.SaveChangesAsync();
    }

    public async Task AddModifier(AddEditModifierViewModel model, string userId)
    {

        var newModifier = new Modifier
        {
            Name = model.Name,
            Rate = model.Rate,
            Quantity = model.Quantity,
            Isdeleted = false,
            Description = string.IsNullOrEmpty(model.Description) ? "" : model.Description,
            Unitid = model.Unitid,
            Createdby = userId,
            Createddate = DateTime.Now,
            Modifiergroupid = model.Modifiergroupid
        };

        await _context.Modifiers.AddAsync(newModifier);
        await _context.SaveChangesAsync();
    }

    public async Task<int> isModifierExist(string name,int? modGrpId)
    {
        var modifier = await _context.Modifiers.Where(m => m.Isdeleted == false && m.Name.ToLower() == name.ToLower() && m.Modifiergroupid == modGrpId).ToListAsync();
        return modifier.Count();
    }

    public async Task<bool> isEditModifierExist(string name,int? id,int? modGrpId)
    {
        var modifier = await _context.Modifiers.Where(m => m.Isdeleted == false && m.Name.ToLower() == name.ToLower() && m.Modifiergroupid == modGrpId).ToListAsync();
        foreach (var m in modifier)
        {
            if(m.Id != id)
                return true;
        }
        return false;
    }

    public async Task EditModifier(AddEditModifierViewModel model,string userId)
    {
        var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == model.Id);
        modifier.Name = model.Name;
        modifier.Rate = model.Rate;
        modifier.Quantity = model.Quantity;
        modifier.Description = string.IsNullOrEmpty(model.Description) ? "" : model.Description;
        modifier.Modifiergroupid = model.Modifiergroupid;
        modifier.Unitid = model.Unitid;
        modifier.Updatedby = userId;
        modifier.Updateddate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task<Modifier?> GetModifierById(int id)
    {
        var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == id);
        return modifier;
    }
}
