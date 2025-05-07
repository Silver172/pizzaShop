using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize(Roles = "Admin,Account Manager")]
public class WaitingListOrderAppController : Controller
{
    private readonly IWaitingListOrderAppService _waitingListService;
    private readonly IJWTService _jwtService;
    private readonly IRolePermissionService _rolePermissionService;
    public WaitingListOrderAppController(IWaitingListOrderAppService waitingListService, IJWTService jwtService,IRolePermissionService rolePermissionService)
    {
        _waitingListService = waitingListService;
        _jwtService = jwtService;
        _rolePermissionService = rolePermissionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        WaitingListOrderAppViewModel waitingListPage = new WaitingListOrderAppViewModel();
        try
        {
            var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var permission = await _rolePermissionService.GetPermissionByRole(roleName);
            HttpContext.Session.SetString("permission",JsonConvert.SerializeObject(permission));
            waitingListPage = await _waitingListService.GetWaitingListPage(0);
            return View(waitingListPage);
        }
        catch (System.Exception)
        {
            return View(waitingListPage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetWaitingListBySectionId(int? id)
    {
        WaitingListOrderAppViewModel waitingListPage = new WaitingListOrderAppViewModel();
        try
        {
            waitingListPage = await _waitingListService.GetWaitingListPage(id);
            return PartialView("_WaitingList", waitingListPage.WaitingList);
        }
        catch (System.Exception)
        {
            return PartialView("_WaitingList", waitingListPage.WaitingList);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSectionList()
    {
        List<SectionViewModel> sectionList = new List<SectionViewModel>();
        try
        {
            sectionList = await _waitingListService.GetSectionList();
            return PartialView("_SectionList", sectionList);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while fetching the data" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddEditWaitingToken(int? id)
    {
        AddEditWaitingTokenViewModel WaitingToken = await _waitingListService.GetAddEditWaitingToken(id);
        return PartialView("_AddEditWaitingTokenModal", WaitingToken);
    }

    [HttpPost]
    public async Task<IActionResult> AddEditWaitingToken(AddEditWaitingTokenViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { isValid = false, message = "fill required field" });
        }

        string authToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(authToken);
        string msg;
        try
        {
            bool Wt = await _waitingListService.AddEditWaitingToken(model, userId);
            msg = "Waiting token updated successfully";
            if (model.Id == 0 || model.Id == null)
                msg = "Waiting token created successfully";
            return Json(new { isSuccess = true, message = msg });
        }
        catch (System.Exception)
        {
            msg = "Error while update waiting token";
            if (model.Id == 0 || model.Id == null)
                msg = "Error while create waiting token";
            return Json(new { isSuccess = false, message = msg });
        }

    }

    [HttpPost]
    public async Task<IActionResult> DeleteWaitingToken(int id)
    {
        string authToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(authToken);
        try
        {
            await _waitingListService.DeleteWaitingtoken(id, userId);
            return Json(new { isSuccess = true, message = "Waiting token deleted SuccessFully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Waiting token deleted SuccessFully" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerSuggestion(string? email)
    {
        try
        {
            List<CustomerSuggestionList> customerSuggestions = await _waitingListService.SearchCustomerByEmail(email);
            return Json(new { isSuccess = true, customerSuggestions });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false });
        }
    }

    [HttpGet]
    public async Task<IActionResult> AssignWaitingToken(){
        try
        {
            AddEditWaitingTokenViewModel model = await _waitingListService.GetAddEditWaitingToken(0);
            return PartialView("_AssignTableModal",model);
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false , message = "Error while fetching the data"});
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTables(int id)
    {
        try
        {
            JsonArray tables = await _waitingListService.GetTablesBySectionId(id);
            return Json(new { isSuccess = true, tables });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false , message = "Error while fetching the data"});
        }
    } 
}
