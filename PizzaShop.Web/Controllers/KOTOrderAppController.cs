using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize(Roles = "Admin,Account Manager,Chef")]
public class KOTOrderAppController : Controller
{
    private IKOTOrderAppService _kotOrderAppService;
    private readonly IJWTService _jwtService;
    private readonly IRolePermissionService _rolePermissionService;
    public KOTOrderAppController(IKOTOrderAppService kotOrderAppService,IJWTService jwtService,IRolePermissionService rolePermissionService)
    {
        _kotOrderAppService = kotOrderAppService;
        _jwtService = jwtService;
        _rolePermissionService = rolePermissionService;
    }
    public async Task<IActionResult> Index()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        
        OrderAppKOTViewModel model = new OrderAppKOTViewModel();
        try
        {
            var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var permission = await _rolePermissionService.GetPermissionByRole(roleName);
            HttpContext.Session.SetString("permission",JsonConvert.SerializeObject(permission));
            model = await _kotOrderAppService.GetKotData(0);
            return View(model);
        }
        catch (System.Exception)
        {
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetKOTByCategoryId(int categoryId, int pageIndex, string? filterBy)
    {
        OrderAppKOTViewModel model = new OrderAppKOTViewModel();
        try
        {
            model = await _kotOrderAppService.GetKOTByCategoryId(categoryId,pageIndex, filterBy);
            return PartialView("_KOT",model);
        }
        catch (System.Exception)
        {
            return View(model);
        }
    }


    [HttpGet]
     public async Task<IActionResult> GetOrderDetails(string data)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        var model = System.Text.Json.JsonSerializer.Deserialize<OrderAppKOTContentViewModel>(data, options);
        return PartialView("_MarkItemStatusModal", model);
    }

    [HttpPost]
    public async Task<IActionResult> MarkOrderItemStatus(string orderItems,string filterBy)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        var model = System.Text.Json.JsonSerializer.Deserialize<List<OrderItemViewModel>>(orderItems, options);

        string authToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(authToken);
        try
        {
            bool result = await _kotOrderAppService.MarkOrderItemStatus(model, userId , filterBy);
            return Json(new { success = true , message= "Order items marked as prepared successfully."});
        }
        catch (System.Exception)
        {
            return Json(new { success = false , message = "Error while marking order items."});
        }
    }
}
