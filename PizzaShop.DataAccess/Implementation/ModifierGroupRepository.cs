using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class ModifierGroupRepository : IModifierGroupRepository
{
    private readonly PizzashopContext _context;

    public ModifierGroupRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<ModifierGroupViewModel>> GetAllModifierGroup()
    {
        var modifierGroups = await _context.Modifiergroups.Where(m => m.Isdeleted == false).OrderBy(m => m.Id).ToListAsync();
        var modifierGroupList = new List<ModifierGroupViewModel>();
        if (modifierGroups.Count > 0 && modifierGroups != null)
        {
            foreach (var modifier in modifierGroups)
            {
                modifierGroupList.Add(new ModifierGroupViewModel
                {
                    Id = modifier.Id,
                    Name = modifier.Name,
                    Description = modifier.Description,
                });
            }
            return modifierGroupList;
        }
        return modifierGroupList;
    }

    public async Task<bool> IsModifierGrpExist(string name)
    {
        var modGrp = await _context.Modifiergroups.FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower() && m.Isdeleted == false);
        if (modGrp == null)
            return false;
        return true;
    }

    public async Task<bool> IsEditModifierGrpExist(string name,int? id)
    {
        var modifierGrp = await _context.Modifiergroups.Where(m => m.Isdeleted == false && m.Name.ToLower() == name.ToLower()).ToListAsync();
        foreach (var m in modifierGrp)
        {
            if(m.Id != id)
                return true;
        }
        return false;
    }

    public async Task<Modifiergroup?> AddModifierGroup(string name, string description, string userId)
    {
        var neweModifierGroup = new Modifiergroup
        {
            Name = name.Trim(),
            Description = string.IsNullOrEmpty(description) ? "" : description.Trim(),
            Createdby = userId,
            Createddate = DateTime.Now,
            Isdeleted = false
        };

        await _context.Modifiergroups.AddAsync(neweModifierGroup);
        await _context.SaveChangesAsync();
        return neweModifierGroup;
    }

    public async Task<Modifiergroup?> GetModifierGroupById(int id)
    {
        var modGrp = await _context.Modifiergroups.FirstOrDefaultAsync(m => m.Id == id);
        return modGrp;
    }

    public async Task EditModifierGroup(int? id, string name, string description, string userId)
    {
        var modifierGroup = await _context.Modifiergroups.FirstOrDefaultAsync(m => m.Id == id);
        modifierGroup.Name = name.Trim();
        modifierGroup.Description = string.IsNullOrEmpty(description) ? "" : description.Trim();
        modifierGroup.Updateddate = DateTime.Now;
        modifierGroup.Updatedby = userId;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteModifierGroup(int id)
    {
        var modifierGroup = await _context.Modifiergroups.FirstOrDefaultAsync(m => m.Id == id);
        modifierGroup.Isdeleted = true;
        await _context.SaveChangesAsync();
    }
}
