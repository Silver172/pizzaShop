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
[PermissionAuthorize("TaxFees.View")]
public class TaxesAndFeesController:Controller
{
    private readonly ITaxesAndFeesService _tnfService;
    private readonly IJWTService _jwtService;
    private readonly IRolePermissionService _rolePermissionService;
    public TaxesAndFeesController(ITaxesAndFeesService tnfService,IJWTService jwtService,IRolePermissionService rolePermissionService)
    {
        _tnfService = tnfService;
        _jwtService = jwtService;
        _rolePermissionService = rolePermissionService;
    }

    [PermissionAuthorize("TaxFees.View")]
    [HttpGet]
    public async Task<IActionResult> Index(int pageIndex=1, int pageSize=5, string? searchString="")
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
        var pagination = new PaginationViewModel{
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchString = searchString
        };
        var tnfList = await _tnfService.GetTexesAndFees(pagination);
        var tr = await _tnfService.GetTaxesAndFeesCount(searchString);
        pagination.TotalRecord = tr;
        pagination.TotalPage = (int)Math.Ceiling(tr / (double)pagination.PageSize);
        var tnfPage = new TaxesAndFeesPageViewModel();
        
        tnfPage.Pagination = pagination;
        tnfPage.TaxesAndFeesList = tnfList;
        return View(tnfPage);
    }

    [HttpGet]
    public async Task<IActionResult> GetTaxesAndFees(int pageIndex, int pageSize, string? searchString)
    {
        var pagination = new PaginationViewModel{
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchString = searchString
        };
        var tnfList = await _tnfService.GetTexesAndFees(pagination);
        var tr = await _tnfService.GetTaxesAndFeesCount(searchString);
        pagination.TotalRecord = tr;
        pagination.TotalPage = (int)Math.Ceiling(tr / (double)pagination.PageSize);
        var tnfPage = new TaxesAndFeesPageViewModel();
        
        tnfPage.Pagination = pagination;
        tnfPage.TaxesAndFeesList = tnfList;
        return PartialView("_TaxesNFeesList",tnfPage);
    }

    [PermissionAuthorize("TaxFees.AddEdit")]
    [HttpGet]
    public async Task<IActionResult> AddEditTax(int? id)
    {
        if(id == null || id == 0)
        {
            var model = new AddEditTaxesAndFeesViewModel();
            return PartialView("_AddEditTaxesAndFees", model);
        }else{
            var taxDetail = await _tnfService.GetTaxDetails(id);
            return PartialView("_AddEditTaxesAndFees", taxDetail);
        }
    }

    [PermissionAuthorize("TaxFees.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> AddEditTax(AddEditTaxesAndFeesViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { isValid = false, message = "Please fill the required fields" });
        }

        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Authentication", "Login");
        
        if(model.Id == 0 || model.Id == null)
        {
            try
            {
                bool isAdded = await _tnfService.AddNewTax(model,userId);
                if(isAdded)
                    return Json(new { isSuccess = true, message = "Tax added successfully" });
                return Json(new { isExist = true, message = "Tax is already exist" });
            }
            catch (System.Exception)
            {
                return Json(new { isSuccess = false, message = "Error while adding new tax" });
            }
        }else{
            try
            {
                bool isEdited = await _tnfService.EditTax(model,userId);
                if(isEdited)
                    return Json(new { isSuccess = true, message = "Tax updated successfully" });
                return Json(new { isExist = true, message = "Tax is already exist" });
            }
            catch (System.Exception)
            {
                return Json(new { isSuccess = false, message = "Error while updating new tax" });
            }
        }
    }

    [PermissionAuthorize("TaxFees.Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteTax(int id)
    {
        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        try
        {
            await _tnfService.DeleteTax(id,userId);
            return Json(new { isSuccess = true, message = "Tax Deleted successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error while deleting tax" });
        }
    }
}
