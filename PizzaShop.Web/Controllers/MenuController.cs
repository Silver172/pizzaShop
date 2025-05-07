using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Authorization;

namespace PizzaShop.Web.Controllers;

[Authorize]
[PermissionAuthorize("Menu.View")]
public class MenuController : Controller
{

    private readonly IMenuService _menuService;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IJWTService _jwtService;
    private readonly IModifierReposotory _modifierRepository;
    private readonly IRolePermissionService _rolePermissionService;
    public MenuController(IMenuService menuService, IMenuItemRepository menuItemRepository, IJWTService jwtService, IModifierReposotory modifierRepository, IRolePermissionService rolePermissionService)
    {
        _menuService = menuService;
        _menuItemRepository = menuItemRepository;
        _jwtService = jwtService;
        _modifierRepository = modifierRepository;
        _rolePermissionService = rolePermissionService;
    }

    [PermissionAuthorize("Menu.View")]
    [HttpGet]
    public async Task<IActionResult> Menu(int pageSize = 5, int pageIndex = 1, string? searchString = "")
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";

        var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var permission = await _rolePermissionService.GetPermissionByRole(roleName);
        HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));
        var AuthToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(AuthToken))
            return RedirectToAction("Login", "Authentication");

        ItemTabViewModel MenuItemTab = await _menuService.GetCategoryItem(pageSize, pageIndex, searchString);
        var pagination = new PaginationViewModel
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchString = searchString
        };
        ModifierTabViewModel modifierTab = await _menuService.GetModifierTab(pagination);
        var model = new MenuViewModel
        {
            ItemsTab = MenuItemTab,
            ModifierTab = modifierTab
        };

        return View(model);
    }

    [PermissionAuthorize("Menu.View")]
    [HttpGet]
    public async Task<IActionResult> GetItemsByCategoryId(int id, int pageSize, int pageIndex, string? searchString)
    {
        List<ItemsViewModel> Items = await _menuService.GetItemsByCategoryId(id, pageSize, pageIndex, searchString);
        var tr = await _menuItemRepository.GetItemsCountByCId(id, searchString);
        var ItemTab = await _menuService.GetCategoryItem(pageSize, pageIndex, searchString);

        var model = new ItemTabViewModel
        {
            ListItems = Items,
            ListCategories = ItemTab.ListCategories,
            AddEditItem = ItemTab.AddEditItem,
            PageSize = pageSize,
            PageIndex = pageIndex,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(tr / (double)pageSize),
            TotalRecord = tr
        };

        return PartialView("_ListItems", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var categories = await _menuService.GetAllCategories();
            return PartialView("_ListCategories", categories);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while fetching categories" });
        }
    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpGet]
    public async Task<IActionResult> AddEditCategory(int? id)
    {
        if (id == null)
        {
            var modal = new AddEditCategoryViewModel();
            return PartialView("_AddEditCategoryModal", modal);
        }
        else
        {
            AddEditCategoryViewModel model = await _menuService.GetCategoryById((int)id);
            return PartialView("_AddEditCategoryModal", model);
        }
    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> AddEditCategory(AddEditCategoryViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { isValid = false, message = "Please fill the required fields" });
        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Authentication", "Login");
        if (model.Id == 0 || model.Id == null)
        {
            try
            {
                bool newCategory = await _menuService.AddCategory(model, userId);
                if (newCategory == false)
                    return Json(new { isExist = true, message = "Category is already exist" });
                return Json(new { isSuccess = true, message = "New category added successfully" });
            }
            catch (System.Exception)
            {
                return Json(new { isSuccess = false, message = "Error while adding new category" });

            }
        }
        else
        {
            try
            {
                var category = await _menuService.EditCategory(model, userId);
                if (category == false)
                    return Json(new { isExist = true, message = "Category is already exist" });
                return Json(new { isSuccess = true, message = "Category updated successfully" });
            }
            catch (System.Exception)
            {
                return Json(new { isSuccess = false, message = "Error while updating category" });

            }
        }
    }

    [PermissionAuthorize("Menu.Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        try
        {
            await _menuService.DeleteCategory(id, userId);
            return Json(new { isSuccess = true, message = "Category deleted successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while deleting category" });
        }
    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpGet]
    public async Task<IActionResult> AddMenuItem()
    {
        try
        {
            AddEditItemViewModel model = await _menuService.GetEmptyMenuItemModal();
            return PartialView("_AddMenuItemModal", model);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while fetching data" });
        }

    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> AddMenuItem(AddEditItemViewModel model, string ItemModifiers)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { isValid = false, message = "Please fill all required field" });
        }
        var AuthToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(AuthToken))
            return RedirectToAction("Login", "Authentication");

        var (userEmail, role, userId) = _jwtService.ValidateToken(AuthToken);
        try
        {
            bool isItemExist = await _menuService.IsItemExist(model.Name, model.CategoryId);
            if (isItemExist)
            {
                return Json(new { isExist = true, message = "This item is already exist" });
            }

            List<ItemModifierViewModel> itemModifiers = new List<ItemModifierViewModel>();
            if (!string.IsNullOrEmpty(ItemModifiers))
            {
                itemModifiers = System.Text.Json.JsonSerializer.Deserialize<List<ItemModifierViewModel>>(ItemModifiers);
            }
            model.ItemModifiers = itemModifiers;

            var response = await _menuService.AddItem(model, userId);
            return Json(new { isSuccess = true, message = "Item added successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while add new item" });
        }
    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpGet]
    public async Task<IActionResult?> EditMenuItem(int itemId)
    {
        var menuItem = await _menuService.GetMenuItemById(itemId);
    
        return PartialView("_EditMenuItemModal", menuItem);
    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpPost]
    public async Task<IActionResult?> EditMenuItem([FromForm] AddEditItemViewModel model, string ItemModifiers)
    {
        var authToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(authToken))
            return null;
        var (email, role, userId) = _jwtService.ValidateToken(authToken);
        if (email == null)
            return null;
        try
        {
            List<ItemModifierViewModel> itemModifiers = new List<ItemModifierViewModel>();
            if (!string.IsNullOrEmpty(ItemModifiers))
            {
                itemModifiers = System.Text.Json.JsonSerializer.Deserialize<List<ItemModifierViewModel>>(ItemModifiers);
            }
            model.ItemModifiers = itemModifiers;
            await _menuService.EditItem(model, userId);
            return Json(new { isSuccess = true, message = "Item updated successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while edit item" });
        }
    }

    [PermissionAuthorize("Menu.Delete")]
    public async Task<IActionResult?> DeleteMenuItem(int id)
    {
        var authToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(authToken))
            return null;
        var (email, role, userId) = _jwtService.ValidateToken(authToken);
        if (email == null)
            return null;
        try
        {
            await _menuService.DeleteMenuItem(id, userId);
            return Json(new { isSuccess = true, message = "Item deleted successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while delete item" });
        }
    }

    [PermissionAuthorize("Menu.Delete")]
    public async Task<IActionResult?> DeleteMultipleMenuItem(int[] ItemIds)
    {
        var authToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(authToken))
            return null;
        var (email, role, userId) = _jwtService.ValidateToken(authToken);
        if (email == null)
            return null;
        try
        {
            foreach (var item in ItemIds)
            { await _menuService.DeleteMenuItem(item, userId); }
            return Json(new { isSuccess = true, message = "Items deleted successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while delete items" });
        }
    }

    // Modifier Tab controllers
    [HttpGet]
    public async Task<IActionResult> GetModifiersByModGrpId(int id, int pageSize, int pageIndex, string? searchString)
    {
        var pagination = new PaginationViewModel
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchString = searchString
        };
        try
        {
            List<ModifierViewModel>? modifierList = await _menuService.GetModifiersByModifierGroupId(id, pagination);
            var modifierTab = new ModifierTabViewModel();
            var tr = await _menuService.GetModifiersCountByMdfGrpId(id, pagination.SearchString);
            if (modifierList != null && modifierList.Count > 0)
            {
                modifierTab.ModifierList = modifierList;
            }
            var mTab = await _menuService.GetModifierTab(pagination);

            modifierTab.AllModifiers = mTab.AllModifiers;
            modifierTab.AddEditModifier = mTab.AddEditModifier;
            modifierTab.Pagination = new PaginationViewModel
            {
                PageIndex = pagination.PageIndex,
                PageSize = pagination.PageSize,
                SearchString = pagination.SearchString,
                TotalPage = (int)Math.Ceiling(tr / (double)pagination.PageSize),
                TotalRecord = tr
            };
            return PartialView("_ListModifiers", modifierTab);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while fetching data" });
        }
    }

    [PermissionAuthorize("Menu.View")]
    [HttpGet]
    public async Task<IActionResult> GetModifiersByModifierGroupId(int id, string name)
    {
        try
        {
            var modifiers = await _menuService.GetModifiersByModifierGroup(id);

            var ItemModifier = new ItemModifierViewModel
            {
                ModifierGroupId = id,
                Name = name,
                ModifierList = modifiers
            };
            return PartialView("_ListModalModifiers", ItemModifier);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while fetching data" });
        }
    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpPost]
    public async Task<IActionResult?> AddModifierGroup([FromBody] AddEditModifierGroupViewModel model)
    {
        var AuthToken = Request.Cookies["AuthToken"];

        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return null;

        try
        {
            bool isAlreadyExist = await _menuService.IsModifierGrpExist(model.Name);
            if (isAlreadyExist)
                return Json(new { isExist = true, message = "Modifier group is already exist" });

            bool IsAdded = await _menuService.AddModifierGroup(model, userId);

            var pagination = new PaginationViewModel
            {
                PageIndex = 1,
                PageSize = 5,
                SearchString = ""
            };
            var modifierTab = await _menuService.GetModifierTab(pagination);
            if (IsAdded)
            {
                return PartialView("_ListModifierGroups", modifierTab);
            }
            else
            {
                return BadRequest();
            }
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while adding new modifier group" });
        }
    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpGet]
    public async Task<IActionResult> EditModifierGroup(int id)
    {
        var AuthToken = Request.Cookies["AuthToken"];

        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login", "Authentication");

        try
        {
            var editModifier = await _menuService.GetEditModifierGroupDetail(id);
            return Json(new { data = editModifier });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while fetching data" });
        }
    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> EditModifierGroup([FromBody] AddEditModifierGroupViewModel model)
    {
        var AuthToken = Request.Cookies["AuthToken"];

        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login", "Authentication");

        try
        {
            bool isAlreadyExist = await _menuService.IsEditModifierGrpExist(model.Name,model.Id);
            if (isAlreadyExist)
                return Json(new { isExist = true, message = "Modifier group is already exist" });
                
            bool isUpdated = await _menuService.EditModifierGroup(model, userId);
            var pagination = new PaginationViewModel { PageIndex = 1, PageSize = 5, SearchString = "" };
            var modifierTab = await _menuService.GetModifierTab(pagination);
            if (isUpdated)
            {
                return PartialView("_ListModifierGroups", modifierTab);
            }
            else
            {
                return BadRequest();
            }
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while edit modifier group" });
        }
    }

    [PermissionAuthorize("Menu.Delete")]
    public async Task<IActionResult> DeleteModifierGroup(int id)
    {
        var AuthToken = Request.Cookies["AuthToken"];

        try
        {
            var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
            var isDeleted = await _menuService.DeleteModifierGroup(id, userId);

            if (isDeleted)
            {
                var pagination = new PaginationViewModel { PageIndex = 1, PageSize = 5, SearchString = "" };
                var modifierTab = await _menuService.GetModifierTab(pagination);
                return PartialView("_ListModifierGroups", modifierTab);
            }
            else
            {
                return BadRequest();
            }
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while delete modifier group" });
        }
    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> AddModifier(AddEditModifierViewModel model)
    {
        var AuthToken = Request.Cookies["AuthToken"];

        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if(!ModelState.IsValid)
            return Json(new { isValid = false, message = "Invalid inputs" });
        try
        {
            bool isAdded = await _menuService.AddModifier(model, userId);

            if (isAdded)
            {
                return Json(new { isSuccess = true, message = "New modifier added successfully" });
            }
            else
                return Json(new { isSuccess = false, message = "Modifier already exist" });  //if null is Modifier Exist
        }
        catch (System.Exception e)
        {
            return Json(new { isSuccess = false, message = "Error while add new modifier" });
        }

    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpGet]
    public async Task<IActionResult> EditModifier(int id)
    {
        try
        {
            var editModifier = await _menuService.GetModifierByid(id);
            return PartialView("_EditModifierModal", editModifier);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while fetching data" });
        }

    }

    [PermissionAuthorize("Menu.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> EditModifier(AddEditModifierViewModel model)
    {
        if(!ModelState.IsValid)
            return Json(new { isValid = false, message = "Invalid inputs" });

        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return null;
        try
        {
            bool isEdited = await _menuService.EditModifier(model, userId);
            if (isEdited)
                return Json(new { isSuccess = true, message = "Modifier updated successfully" });
            else
                return Json(new { isSuccess = false, message = "Modifier is already exist" });
        }
        catch (System.Exception e)
        {
            return Json(new { isSuccess = false, message = "Error while edit modifier" });
        }

    }

    [PermissionAuthorize("Menu.Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteModifier(int id)
    {
        var AuthToken = Request.Cookies["AuthToken"];

        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        try
        {
            await _menuService.DeleteModifier(id, userId);
            return Json(new { isSuccess = true, message = "Modifier deleted successfully" });
        }
        catch (System.Exception e)
        {
            return Json(new { isSuccess = false, isAuthenticate = true, message = "Error while delete modifier" });
        }
    }

    [PermissionAuthorize("Menu.Delete")]
    public async Task<IActionResult> DeleteMultipleModifier(int[] modifierIds)
    {
        var AuthToken = Request.Cookies["AuthToken"];

        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        try
        {
            await _menuService.DeleteMultipleModifiers(modifierIds, userId);
            return Json(new { isSuccess = true, message = "Modifiers deleted successfully" });
        }
        catch (System.Exception e)
        {
            return Json(new { isSuccess = true, message = "Error while delete Modifiers" });
        }
    }
}
