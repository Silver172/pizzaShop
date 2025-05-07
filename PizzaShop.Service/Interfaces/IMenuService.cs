using Microsoft.AspNetCore.Mvc;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IMenuService
{
    public Task<ItemTabViewModel> GetCategoryItem(int pageSize, int pageIndex, string? searchString);
    public Task<bool> AddCategory(AddEditCategoryViewModel model, string email);
    public Task<AddEditCategoryViewModel?> GetCategoryById(int id);
    public Task<List<CategoriesViewModel>> GetAllCategories();
    public Task<bool> EditCategory(AddEditCategoryViewModel model, string email);
    public Task<bool> DeleteCategory(int id, string userId);
    public Task<List<ItemsViewModel>?> GetItemsByCategoryId(int id, int pageSize, int pageIndex, string? searchString);
    public Task<bool> AddItem(AddEditItemViewModel model, string userId);
    public Task<AddEditItemViewModel> GetMenuItemById(int id);
    public Task<bool> IsItemExist(string name, int catId);
    public Task EditItem(AddEditItemViewModel model, string userId);
    public Task DeleteMenuItem(int id,string userId);
    public Task<int> GetItemsCountByCId(int cId, string? searchString);
    public Task<AddEditItemViewModel> GetEmptyMenuItemModal();

    public Task<ModifierTabViewModel> GetModifierTab(PaginationViewModel pagination);
    public Task<List<ModifierViewModel>?> GetModifiersByModifierGroupId(int id, PaginationViewModel pagination);
    public Task<int> GetModifiersCountByMdfGrpId(int id, string? searchString);
    public Task<bool> IsModifierGrpExist(string name);
    public Task<bool> IsEditModifierGrpExist(string name,int? id);
    public Task<ModifierGroupViewModel> GetModifierGroupById(int id);
    public Task<bool> AddModifierGroup(AddEditModifierGroupViewModel model, string userId);
    public Task<bool> EditModifierGroup(AddEditModifierGroupViewModel model, string userId);
    public Task<List<ModifierGroupViewModel>?> GetAllModifierGroups();
    public Task<List<ModifierViewModel>?> GetModifiersByModifierGroup(int id);
    public Task<AddEditModifierGroupViewModel> GetEditModifierGroupDetail(int id);
    public Task<bool> DeleteModifierGroup(int id,string userId);
    public Task<bool> AddModifier(AddEditModifierViewModel model, string userId);
    public Task<bool> EditModifier(AddEditModifierViewModel model, string userId);
    public Task DeleteModifier(int id,string userId);
    public Task<AddEditModifierViewModel> GetModifierByid(int id);
    public Task DeleteMultipleModifiers(int[] modifierIds,string userId);
}
