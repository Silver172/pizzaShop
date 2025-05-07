using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IModifierGroupRepository
{
    public Task<List<ModifierGroupViewModel>> GetAllModifierGroup();
    public Task<bool> IsModifierGrpExist(string name);
    public Task<Modifiergroup?> AddModifierGroup(string name, string description, string userId);
    public Task<Modifiergroup?> GetModifierGroupById(int id);
    public Task EditModifierGroup(int? id, string name, string description, string userId);
    public Task DeleteModifierGroup(int id);
    public Task<bool> IsEditModifierGrpExist(string name,int? id);
}
