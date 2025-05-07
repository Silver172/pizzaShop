using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class MenuService : IMenuService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IUsersService _userService;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IModifierGroupRepository _modifierGroupRepository;
    private readonly IMappingMenuItemWithModifierRepository _mappingMenuItemRepository;
    private readonly IModifierReposotory _modifierRepository;
    // private readonly IUserRepository _userRepository;
    public MenuService(
        ICategoryRepository categoryRepository,
        IMenuItemRepository menuItemRepository,
        IUsersService userService,
        IRoleRepository roleRepository,
        IUnitRepository unitRepository,
        IModifierGroupRepository modiferGroupRepository,
        IMappingMenuItemWithModifierRepository mappingMenuItem,
        IModifierReposotory modifierRepository
    )
    {
        _categoryRepository = categoryRepository;
        _menuItemRepository = menuItemRepository;
        _userService = userService;
        _roleRepository = roleRepository;
        _unitRepository = unitRepository;
        _modifierGroupRepository = modiferGroupRepository;
        _mappingMenuItemRepository = mappingMenuItem;
        _modifierRepository = modifierRepository;
    }

    public async Task<ItemTabViewModel> GetCategoryItem(int pageSize, int pageIndex, string? searchString)
    {
        List<Category> Categories = await _categoryRepository.GetAllCategories();

        var categoryList = new List<CategoriesViewModel>();

        foreach (var cat in Categories)
        {
            categoryList.Add(new CategoriesViewModel
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description,
            });
        }

        var MenuItem = await _menuItemRepository.GetMenuItemsByCategoryId(Categories[0].Id, pageSize, pageIndex, searchString);
        var itemList = new List<ItemsViewModel>();
        foreach (var item in MenuItem)
        {
            itemList.Add(new ItemsViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Itemtype = item.Itemtype,
                Rate = item.Rate,
                Quantity = item.Quantity,
                Isavailable = item.Isavailable,
                Itemimage = item.Itemimage
            });
        }

        var tr = await _menuItemRepository.GetItemsCountByCId(Categories[0].Id, searchString);
        var units = await _unitRepository.GetUnits();
        var modifierGroups = await _modifierGroupRepository.GetAllModifierGroup();

        var AddEditItem = new AddEditItemViewModel
        {
            Categories = categoryList,
            Units = units,
            ModifierGroups = modifierGroups
        };

        var model = new ItemTabViewModel
        {
            ListCategories = categoryList,
            ListItems = itemList,
            AddEditItem = AddEditItem,
            PageSize = pageSize,
            PageIndex = pageIndex,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(tr / (double)pageSize),
            TotalRecord = tr
        };
        return model;
    }


    public async Task<List<CategoriesViewModel>> GetAllCategories()
    {
        List<Category> Categories = await _categoryRepository.GetAllCategories();

        var categoryList = new List<CategoriesViewModel>();

        foreach (var cat in Categories)
        {
            categoryList.Add(new CategoriesViewModel
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description,
            });
        }
        return categoryList;
    }

    public async Task<int> GetItemsCountByCId(int cId, string? searchString)
    {
        return await _menuItemRepository.GetItemsCountByCId(cId, searchString);
    }

    public async Task<bool> AddCategory(AddEditCategoryViewModel model, string userId)
    {
        int count = await _categoryRepository.IsCategoryExist(model.Name);
        if (count > 0)
            return false;
        return await _categoryRepository.AddCategory(model, userId);
    }

    public async Task<AddEditCategoryViewModel?> GetCategoryById(int id)
    {
        var category = await _categoryRepository.GetCategoryById(id);
        var model = new AddEditCategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
        return model;
    }

    public async Task<bool> EditCategory(AddEditCategoryViewModel model, string userId)
    {
        bool isExist = await _categoryRepository.IsEditCategoryExist(model.Name, model.Id);
        if (isExist)
            return false;
        return await _categoryRepository.EditCategory(model, userId);
    }

    public async Task<List<ItemsViewModel>?> GetItemsByCategoryId(int id, int pageSize, int pageIndex, string? searchString)
    {
        var items = await _menuItemRepository.GetMenuItemsByCategoryId(id, pageSize, pageIndex, searchString);
        var itemList = new List<ItemsViewModel>();

        if (items.Count > 0)
        {
            foreach (var item in items)
            {
                itemList.Add(new ItemsViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Itemtype = item.Itemtype,
                    Rate = item.Rate,
                    Quantity = item.Quantity,
                    Isavailable = item.Isavailable,
                    Isdeleted = item.Isdeleted == null ? false : true,
                    Categoryid = (int)item.Categoryid,
                    Itemimage = item.Itemimage
                });
            }
            return itemList;
        }
        return null;
    }

    public async Task<bool> DeleteCategory(int id, string userId)
    {
        bool deleteCategory = await _categoryRepository.DeleteCategory(id,userId);
        if (deleteCategory)
        {
            bool deleteItems = await _menuItemRepository.DeleteMenuItemsByCategoryId(id,userId);
            if (deleteItems)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> AddItem(AddEditItemViewModel model, string userId)
    {
        string ItemImagePath = null;
        if (model.Itemimage != null && model.Itemimage.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ItemImages");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.Itemimage.FileName);
            var filePath = Path.Combine(folderPath, filename);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.Itemimage.CopyTo(stream);
            }
            ItemImagePath = "/ItemImages/" + filename;
        }
        if (ItemImagePath != null)
            model.ImagePath = ItemImagePath;
        if(model.Quantity == 0)
            model.Isavailable = false;
        Menuitem Item = await _menuItemRepository.AddItem(model, userId);
        if (Item == null)
        {
            return false;
        }
        bool isAddMapping = await _mappingMenuItemRepository.AddMapping(model.ItemModifiers, Item.Id, userId);

        if (isAddMapping)
            return true;
        return false;
    }

    public async Task<AddEditItemViewModel> GetEmptyMenuItemModal()
    {
        var units = await _unitRepository.GetUnits();
        var modifierGroups = await _modifierGroupRepository.GetAllModifierGroup();
        List<Category> Categories = await _categoryRepository.GetAllCategories();
        var categoryList = new List<CategoriesViewModel>();
        foreach (var cat in Categories)
        {
            categoryList.Add(new CategoriesViewModel
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description,
            });
        }
        AddEditItemViewModel model = new AddEditItemViewModel{
            Units = units,
            Categories = categoryList,
            ModifierGroups = modifierGroups
        };
        return model;
    }

    public async Task<AddEditItemViewModel> GetMenuItemById(int id)
    {
        Menuitem item = await _menuItemRepository.GetMenuItemById(id);
        var units = await _unitRepository.GetUnits();
        var modifierGroups = await _modifierGroupRepository.GetAllModifierGroup();
        List<Category> Categories = await _categoryRepository.GetAllCategories();
        List<ItemModifierViewModel> ItemModifiersData = await _mappingMenuItemRepository.ModifierGroupDataByItemId(id);

        var categoryList = new List<CategoriesViewModel>();

        foreach (var cat in Categories)
        {
            categoryList.Add(new CategoriesViewModel
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description,
            });
        }
        foreach (var m in ItemModifiersData)
        {
            List<ModifierViewModel>? ML = await GetModifiersByModifierGroup(m.ModifierGroupId);
            m.ModifierList = ML;
        }
        var menuItem = new AddEditItemViewModel
        {
            id = item.Id,
            CategoryId = (int)item.Categoryid,
            Name = item.Name,
            Itemtype = item.Itemtype,
            Rate = item.Rate,
            Quantity = item.Quantity,
            Isavailable = (bool)(item.Isavailable == null ? false : item.Isavailable),
            Isdefaulttax = (bool)(item.Isdefaulttax == null ? false : item.Isdefaulttax),
            Unitid = (int)(item.Unitid == null ? 0 : item.Unitid),
            Taxpercentage = item.Taxpercentage,
            Shortcode = item.Shortcode,
            Description = item.Description,
            ModifierGroups = modifierGroups,
            Units = units,
            ImagePath = item.Itemimage,
            Categories = categoryList,
            ItemModifiers = ItemModifiersData
        };

        return menuItem;
    }

    public async Task<bool> IsItemExist(string name, int catId)
    {
        return await _menuItemRepository.IsItemExist(name, catId);
    }

    public async Task EditItem(AddEditItemViewModel model, string userId)
    {
        string ItemImagePath = null;
        if (model.Itemimage != null && model.Itemimage.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ItemImages");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.Itemimage.FileName);
            var filePath = Path.Combine(folderPath, filename);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.Itemimage.CopyTo(stream);
            }
            ItemImagePath = "/ItemImages/" + filename;
        }
        if (ItemImagePath != null)
            model.ImagePath = ItemImagePath;
        if(model.Quantity == 0)
            model.Isavailable = false;
        await _menuItemRepository.EditMenuItem(model, userId);

        // if (model.ItemModifiers.Count > 0)
        // {
        var oldMapping = await _mappingMenuItemRepository.ModifierGroupDataByItemId(model.id);
        var newMapping = model.ItemModifiers;

        var oldMappingId = new List<int>();
        foreach (var om in oldMapping)
            oldMappingId.Add((int)om.Id);

        var AddMapping = new List<ItemModifierViewModel>();
        var EditMapping = new List<ItemModifierViewModel>();

        foreach (var m in newMapping)
        {
            if (m.Id == -1)
            {
                AddMapping.Add(m);
            }
            else if (oldMappingId.IndexOf((int)m.Id) != -1)
            {
                EditMapping.Add(m);
                oldMappingId.Remove((int)m.Id);
            }
        }
        // also delete mapping (oldMappingId)
        await _mappingMenuItemRepository.DeleteMapping(oldMappingId,userId);
        await _mappingMenuItemRepository.AddMapping(AddMapping, model.id, userId);
        await _mappingMenuItemRepository.EditMappings(EditMapping, model.id, userId);
        // }
    }

    public async Task DeleteMenuItem(int id,string userId)
    {
        await _menuItemRepository.DeleteMenuItem(id,userId);
    }

    public async Task<List<ModifierViewModel>?> GetModifiersByModifierGroup(int id)
    {
        var modifiers = await _modifierRepository.GetModifiersByModifierGroup(id);
        var modifierList = new List<ModifierViewModel>();

        foreach (var modifier in modifiers)
        {
            modifierList.Add(new ModifierViewModel
            {
                Id = modifier.Id,
                Name = modifier.Name,
                Description = modifier.Description,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
                Isdeleted = modifier.Isdeleted,
                Modifiergroupid = modifier.Modifiergroupid,
                Unitid = modifier.Unitid,
                Unit = (await _unitRepository.GetUnitById(modifier.Unitid))?.Name
            });
        }
        return modifierList;
    }

    // Modifier Tab Services
    public async Task<List<ModifierGroupViewModel>?> GetAllModifierGroups()
    {
        List<ModifierGroupViewModel> modifierGroups = await _modifierGroupRepository.GetAllModifierGroup();
        if (modifierGroups.Count > 0 && modifierGroups != null)
        {
            return modifierGroups;
        }
        else
        {
            return null;
        }
    }

    public async Task<List<ModifierViewModel>?> GetAllModifiers()
    {
        var modifiers = await _modifierRepository.GetAllModifiers();
        var allModifiers = new List<ModifierViewModel>();
        if (modifiers != null)
        {
            foreach (var m in modifiers)
            {
                allModifiers.Add(new ModifierViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Rate = m.Rate,
                    Quantity = m.Quantity,
                    Unit = (await _unitRepository.GetUnitById(m.Unitid))?.Name,
                    Unitid = m.Unitid
                });
            }
            return allModifiers;
        }
        return null;
    }
    public async Task<ModifierTabViewModel> GetModifierTab(PaginationViewModel pagination)
    {
        var modifierTab = new ModifierTabViewModel();
        List<ModifierGroupViewModel>? modifierGroupList = await GetAllModifierGroups();
        var modifierList = new List<ModifierViewModel>();
        var AddEditModifier = new AddEditModifierViewModel();
        int tr = 0;


        if (modifierGroupList != null)
        {
            modifierTab.ModifierGroupList = modifierGroupList;

            tr = await _modifierRepository.GetModifiersCountByMdfGrpId(modifierGroupList[0].Id, pagination.SearchString);
            modifierList = await GetModifiersByModifierGroupId(modifierGroupList[0].Id, pagination);
            if (modifierList != null && modifierList.Count > 0)
            {
                modifierTab.ModifierList = modifierList;
            }
        }
        var units = await _unitRepository.GetUnits();
        var modifierGroups = modifierGroupList;
        AddEditModifier.ModifierGroups = modifierGroups;
        AddEditModifier.Units = units;
        List<ModifierViewModel>? allModifiers = await GetAllModifiers();

        modifierTab.AddEditModifier = AddEditModifier;
        modifierTab.AllModifiers = allModifiers;
        modifierTab.Pagination = new PaginationViewModel
        {
            PageIndex = pagination.PageIndex,
            PageSize = pagination.PageSize,
            SearchString = pagination.SearchString,
            TotalPage = (int)Math.Ceiling(tr / (double)pagination.PageSize),
            TotalRecord = tr
        };

        return modifierTab;
    }

    public async Task<List<ModifierViewModel>?> GetModifiersByModifierGroupId(int id, PaginationViewModel pagination)
    {
        List<Modifier>? modifiers = await _modifierRepository.GetModifiersByModifierGroupId(id, pagination);

        var modifierList = new List<ModifierViewModel>();
        if (modifiers != null && modifiers.Count > 0)
        {
            foreach (var modifier in modifiers)
            {
                modifierList.Add(new ModifierViewModel
                {
                    Id = modifier.Id,
                    Name = modifier.Name,
                    Description = modifier.Description,
                    Rate = modifier.Rate,
                    Quantity = modifier.Quantity,
                    Isdeleted = modifier.Isdeleted,
                    Modifiergroupid = modifier.Modifiergroupid,
                    Unitid = modifier.Unitid,
                    Unit = (await _unitRepository.GetUnitById(modifier.Unitid))?.Name
                });
            }
            return modifierList;
        }
        else
        {
            return null;
        }
    }

    public async Task<int> GetModifiersCountByMdfGrpId(int id, string? searchString)
    {
        return await _modifierRepository.GetModifiersCountByMdfGrpId(id, searchString);
    }

    public async Task<bool> IsModifierGrpExist(string name)
    {
        return await _modifierGroupRepository.IsModifierGrpExist(name);
    }

    public async Task<bool> IsEditModifierGrpExist(string name,int? id)
    {
        return await _modifierGroupRepository.IsEditModifierGrpExist(name,id);
    }

    public async Task<ModifierGroupViewModel> GetModifierGroupById(int id)
    {
        var modGrp = await _modifierGroupRepository.GetModifierGroupById(id);
        var modifierGroup = new ModifierGroupViewModel
        {
            Id = modGrp.Id,
            Name = modGrp.Name,
            Description = modGrp.Description
        };
        return modifierGroup;
    }

    public async Task<bool> AddModifierGroup(AddEditModifierGroupViewModel model, string userId)
    {
        try
        {
            var newModifierGroup = await _modifierGroupRepository.AddModifierGroup(model.Name, model.Description, userId);
            if (newModifierGroup == null)
                return false;

            await _modifierRepository.AddExistingModifiers(model.Modifiers, newModifierGroup.Id, userId);

            return true;
        }
        catch (System.Exception e)
        {
            return false;
        }
    }

    public async Task<bool> EditModifierGroup(AddEditModifierGroupViewModel model, string userId)
    {
        var oldModifiers = await _modifierRepository.GetModifiersByModifierGroup(model.Id);

        var oldModifiersId = new List<int>();
        var modifiersId = new List<int>();
        foreach (var om in oldModifiers)
            oldModifiersId.Add(om.Id);
        foreach (var m in model.Modifiers)
            modifiersId.Add(m.Id);

        try
        {
            var modifiers = model.Modifiers;

            foreach (var m in modifiers)
            {
                if (oldModifiersId.IndexOf(m.Id) == -1)
                    await _modifierRepository.AddExistingModifier(m, model.Id, userId);
            }

            foreach (var m in oldModifiers)
            {
                if (modifiersId.IndexOf(m.Id) == -1)
                    await _modifierRepository.DeleteModifier(m.Id,userId);
            }

            await _modifierGroupRepository.EditModifierGroup(model.Id, model.Name, model.Description, userId);
            return true;
        }
        catch (System.Exception e)
        {
            return false;
        }
    }

    public async Task<AddEditModifierGroupViewModel> GetEditModifierGroupDetail(int id)
    {
        var modifiergroup = await _modifierGroupRepository.GetModifierGroupById(id);
        var modifiers = await _modifierRepository.GetModifiersByModifierGroup(id);
        var EditModifierDetail = new AddEditModifierGroupViewModel();

        EditModifierDetail.Id = modifiergroup.Id;
        EditModifierDetail.Name = modifiergroup.Name;
        EditModifierDetail.Description = modifiergroup.Description;

        var ExistingModifiers = new List<ExistingModifierViewModel>();
        if (modifiers.Count > 0)
        {
            foreach (var m in modifiers)
            {
                ExistingModifiers.Add(new ExistingModifierViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                });
            }
        }
        EditModifierDetail.Modifiers = ExistingModifiers;
        return EditModifierDetail;
    }

    public async Task<bool> DeleteModifierGroup(int id,string userId)
    {
        try
        {
            var modifiers = await _modifierRepository.GetModifiersByModifierGroup(id);
            if (modifiers.Count > 0)
            {
                foreach (var m in modifiers)
                    await _modifierRepository.DeleteModifier(m.Id,userId);
            }
            await _modifierGroupRepository.DeleteModifierGroup(id);
            return true;
        }
        catch (System.Exception e)
        {
            return false;
        }

    }

    public async Task<bool> AddModifier(AddEditModifierViewModel model, string userId)
    {
        int isExist = await _modifierRepository.isModifierExist(model.Name, model.Modifiergroupid);
        if (isExist > 0)
            return false;
        await _modifierRepository.AddModifier(model, userId);
        return true;
    }

    public async Task<bool> EditModifier(AddEditModifierViewModel model, string userId)
    {
        bool isExist = await _modifierRepository.isEditModifierExist(model.Name, model.Id, model.Modifiergroupid);
        if (isExist)
            return false;
        await _modifierRepository.EditModifier(model, userId);
        return true;
    }

    public async Task DeleteModifier(int id,string userId)
    {
        await _modifierRepository.DeleteModifier(id,userId);
    }

    public async Task DeleteMultipleModifiers(int[] modifierIds,string userId)
    {
        if (modifierIds.Length > 0)
        {
            foreach (var id in modifierIds)
            {
                await _modifierRepository.DeleteModifier(id,userId);
            }
        }
    }
    public async Task<AddEditModifierViewModel> GetModifierByid(int id)
    {
        var modifier = await _modifierRepository.GetModifierById(id);
        var modifierGroups = await _modifierGroupRepository.GetAllModifierGroup();
        var units = await _unitRepository.GetUnits();

        var editModifier = new AddEditModifierViewModel
        {
            Id = modifier.Id,
            Name = modifier.Name,
            Rate = modifier.Rate,
            Quantity = modifier.Quantity,
            Description = modifier.Description,
            Modifiergroupid = (int)modifier.Modifiergroupid,
            Unitid = modifier.Unitid,
            ModifierGroups = modifierGroups,
            Units = units
        };
        return editModifier;
    }
}
