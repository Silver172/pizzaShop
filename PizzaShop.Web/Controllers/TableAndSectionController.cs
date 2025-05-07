using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Authorization;

namespace PizzaShop.Web.Controllers;

[Authorize]
[PermissionAuthorize("TableSection.View")]
public class TableAndSectionController : Controller
{
    private readonly ITableAndSectionService _tableAndSectionService;
    private readonly IJWTService _jwtService;
    private readonly IRolePermissionService _rolePermissionService;
    public TableAndSectionController(ITableAndSectionService tableAndSectionService, IJWTService jwtService,IRolePermissionService rolePermissionService)
    {
        _tableAndSectionService = tableAndSectionService;
        _jwtService = jwtService;
        _rolePermissionService = rolePermissionService;
    }

    [PermissionAuthorize("TableSection.View")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";

        var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var permission = await _rolePermissionService.GetPermissionByRole(roleName);
        HttpContext.Session.SetString("permission",JsonConvert.SerializeObject(permission));

        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Authentication", "Login");

        var TSPage = await _tableAndSectionService.GetTableSectionPage();
        return View(TSPage);
    }

    [HttpGet]
    public async Task<IActionResult> GetTablesBySectionId(int id, int pageIndex, int pageSize, string? searchString)
    {

        var pagination = new PaginationViewModel
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchString = searchString
        };
        int tr = await _tableAndSectionService.GetTablesCountbySectionId(id,searchString);
        pagination.TotalRecord = tr;
        pagination.TotalPage = (int)Math.Ceiling(tr / (double)pagination.PageSize);

        var TSPage = await _tableAndSectionService.GetTableSectionPage();
        var tableList = await _tableAndSectionService.GetTablesBySectionId(id, pagination);
        TSPage.TableList = tableList;

        TSPage.Pagination = pagination;
        return PartialView("_ListTables", TSPage);
    }

    [PermissionAuthorize("TableSection.AddEdit")]
    [HttpGet]
    public IActionResult AddSection()
    {
        return PartialView("_AddSection", new AddEditSectionViewModel());
    }

    [PermissionAuthorize("TableSection.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> AddSection(AddEditSectionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { isValid = false, message = "Please fill the required fields" });
        }

        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Authentication", "Login");

        try
        {
            var Section = await _tableAndSectionService.AddSection(model, userId);
            if (Section == null)
                return Json(new { isExist = true, message = "Section is already exist" });
            return PartialView("_ListSections", Section);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while adding new section" });

        }
    }

    [PermissionAuthorize("TableSection.AddEdit")]
    [HttpGet]
    public async Task<IActionResult> EditSection(int id)
    {
        try
        {
            AddEditSectionViewModel section = await _tableAndSectionService.GetSectionById(id);
            return PartialView("_EditSection", section);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while fetching sections" });
        }

    }

    [PermissionAuthorize("TableSection.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> EditSection(AddEditSectionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { isValid = false, message = "Please fill the required fields" });
        }
        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);

        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Authentication", "Login");

        try
        {
            var sectionList = await _tableAndSectionService.EditSection(model, userId);
            if (sectionList == null)
                return Json(new { isExist = true, message = "Section name is already exist" });
            return PartialView("_ListSections", sectionList);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while updating section" });
        }
    }

    [PermissionAuthorize("TableSection.Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteSection(int id)
    {
        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        try
        {
            var isDeleted = await _tableAndSectionService.DeleteSection(id,userId);
            if (isDeleted == null)
                return Json(new { isExist = true, message = "This section have occupied table" });
            return PartialView("_ListSections", isDeleted); ;
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while deleting section" });
        }
    }

    [PermissionAuthorize("TableSection.AddEdit")]
    [HttpGet]
    public async Task<IActionResult> AddEditTable(int? id)
    {

        if (id == 0 || id == null)
        {
            var addTable = new AddEditTableViewModel();
            addTable.Sections = await _tableAndSectionService.GetAllSection();
            return PartialView("_AddEditTable", addTable);
        }
        else
        {
            var editTable = await _tableAndSectionService.GetTableById(id);
            return PartialView("_AddEditTable",editTable);
        }
    }

    [PermissionAuthorize("TableSection.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> AddTable(AddEditTableViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { isValid = false, message = "Please fill the required fields" });
        }

        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Authentication", "Login");
        if (model.Id == 0 || model.Id == null)
        {
            try
            {
                var table = await _tableAndSectionService.AddTable(model, userId);
                if (table == false)
                    return Json(new { isExist = true, message = "Table is already exist" });
                return Json(new { isSuccess = true, message = "Table added successfully" });
            }
            catch (System.Exception)
            {
                return Json(new { isSuccess = false, message = "Error while adding new table" });

            }
        }
        else
        {
            try
            {
                var table = await _tableAndSectionService.EditTable(model, userId);
                if (table == false)
                    return Json(new { isExist = true, message = "Table is already exist" });
                return Json(new { isSuccess = true, message = "Table updated successfully" });
            }
            catch (System.Exception)
            {
                return Json(new { isSuccess = false, message = "Error while updating new table" });

            }
        }
    }
    
    [PermissionAuthorize("TableSection.Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteTable(int id)
    {
        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        try
        {    
            bool isDeleted = await _tableAndSectionService.DeleteTable(id,userId);
            if(isDeleted)
                return Json(new { isSuccess = true, message = "Table Deleted successfully" });
            else    
                return Json(new { isSuccess = false, message = "Table is occupied !" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while deleting table" });
        }
    }

    [PermissionAuthorize("TableSection.Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteMultipleTable(int[] ids)
    {
        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        try
        {
            bool isDeleted = await _tableAndSectionService.DeleteMultipleTable(ids,userId);

            if(isDeleted)
                return Json(new { isSuccess = true, message = "Table Deleted successfully" });
            else    
                return Json(new { isSuccess = false, message = "Table is occupied !" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while deleting tables" });
        }
    }

}
