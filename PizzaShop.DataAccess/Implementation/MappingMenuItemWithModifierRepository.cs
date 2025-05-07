using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class MappingMenuItemWithModifierRepository : IMappingMenuItemWithModifierRepository
{
    private readonly PizzashopContext _context;

    public MappingMenuItemWithModifierRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<bool> AddMapping(List<ItemModifierViewModel> modifierGroupData, int itemId, string userId)
    {
        if (modifierGroupData.Count < 1)
            return true;

        foreach (var mgId in modifierGroupData)
        {
            var record = new Mappingmenuitemwithmodifier
            {
                Menuitemid = itemId,
                Modifiergroupid = mgId.ModifierGroupId,
                Minselectionrequired = (short)mgId.Minselectionrequired,
                Maxselectionrequired = (short)mgId.Maxselectionrequired,
                Createddate = DateTime.Now,
                Createdby = userId,
                Isdeleted = false
            };

            await _context.Mappingmenuitemwithmodifiers.AddAsync(record);
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EditMappings(List<ItemModifierViewModel> modifierGroupData, int itemId, string userId)
    {
        foreach (var mgId in modifierGroupData)
        {
            var mapping = await _context.Mappingmenuitemwithmodifiers.FirstOrDefaultAsync(m => m.Id == mgId.Id);
            if (mapping != null)
            {
                mapping.Maxselectionrequired = (short)mgId.Maxselectionrequired;
                mapping.Minselectionrequired = (short)mgId.Minselectionrequired;
                mapping.Updatedby = userId;
                mapping.Updateddate = DateTime.Now;
            }
            else
            {
                return false;
            }
        }
        await _context.SaveChangesAsync();
        return true;
    }


    // public async Task<bool> EditMapping(AddEditItemViewModel model,int ItemId,string role)
    // {
    //     var oldMapping = await _context.Mappingmenuitemwithmodifiers.Where(m => m.Menuitemid == ItemId).ToListAsync();
    //     var newMapping = model.ItemModifiers;

    //     return true;
    // }

    public async Task<List<ItemModifierViewModel>> ModifierGroupDataByItemId(int itemId)
    {
        var mapping = await _context.Mappingmenuitemwithmodifiers.Where(m => m.Menuitemid == itemId && m.Isdeleted == false).ToListAsync();
        var ItemModifiers = new List<ItemModifierViewModel>();
        foreach (var m in mapping)
        {
            ItemModifiers.Add(new ItemModifierViewModel
            {
                Id = m.Id,
                ModifierGroupId = (int)m.Modifiergroupid,
                Name = (await _context.Modifiergroups.FirstOrDefaultAsync(mg => mg.Id == m.Modifiergroupid)).Name,
                Minselectionrequired = m.Minselectionrequired,
                Maxselectionrequired = m.Maxselectionrequired,
            });
        }
        return ItemModifiers;
    }

    public async Task DeleteMapping(List<int> mappingIds, string userId)
    {
        foreach (var m in mappingIds)
        {
            var mapping = await _context.Mappingmenuitemwithmodifiers.FirstOrDefaultAsync(i => i.Id == m);
            mapping.Isdeleted = true;
            mapping.Updatedby = userId;
            mapping.Updateddate = DateTime.Now;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<Mappingmenuitemwithmodifier>> GetMappingsByItemId(int itemId)
    {
        var mapping = await _context.Mappingmenuitemwithmodifiers.Where(m => m.Menuitemid == itemId && m.Isdeleted == false )
        .Include(m => m.Modifiergroup)
        .ThenInclude(m => m.Modifiers.Where(m => m.Isdeleted == false))
        .ToListAsync();
        return mapping;
    }
}
