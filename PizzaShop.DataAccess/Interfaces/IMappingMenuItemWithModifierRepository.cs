using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IMappingMenuItemWithModifierRepository
{
    public Task<bool> AddMapping(List<ItemModifierViewModel> modifierGroupData,int itemId,string userId);
    public Task<bool> EditMappings(List<ItemModifierViewModel> modifierGroupData,int itemId,string userId);
    public Task<List<ItemModifierViewModel>> ModifierGroupDataByItemId(int itemId);
    public Task DeleteMapping(List<int> mappingIds,string userId);
    public Task<List<Mappingmenuitemwithmodifier>> GetMappingsByItemId(int itemId);
}
