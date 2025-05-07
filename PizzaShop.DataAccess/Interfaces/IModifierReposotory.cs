using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IModifierReposotory
{
    public Task<List<Modifier>?> GetModifiersByModifierGroupId(int id, PaginationViewModel pagination);
    public Task<int> GetModifiersCountByMdfGrpId(int id,string? searchString);
    public Task<List<Modifier>?> GetAllModifiers();
    public Task<List<Modifier>?> GetModifiersByModifierGroup(int? id);
    public Task AddExistingModifiers(List<ExistingModifierViewModel> modifiers, int modifierGroupId, string userId);
    public Task AddExistingModifier(ExistingModifierViewModel modifier, int? modifierGroupId, string userId);
    public Task DeleteModifier(int id,string userId);
    public Task<int> isModifierExist(string name,int? modGrpId);
    public Task AddModifier(AddEditModifierViewModel model, string userId);
    public Task EditModifier(AddEditModifierViewModel model,string userId);
    public Task<Modifier?> GetModifierById(int id);
    public Task<bool> isEditModifierExist(string name,int? id,int? modGrpId);
    
}
