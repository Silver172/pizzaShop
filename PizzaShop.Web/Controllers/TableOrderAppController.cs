using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize(Roles = "Admin,Account Manager")]
public class TableOrderAppController : Controller
{
    private readonly IRolePermissionService _rolePermissionService;
    private readonly ITableOrderAppService _tableOrderAppService;
    private readonly IJWTService _jwtService;
    private readonly IOrderRepository _orderRepository;
    
    public TableOrderAppController(IRolePermissionService rolePermissionService, ITableOrderAppService tableOrderAppService, IJWTService jWTService, IOrderRepository orderRepository)
    {
        _rolePermissionService = rolePermissionService;
        _tableOrderAppService = tableOrderAppService;
        _jwtService = jWTService;
        _orderRepository = orderRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        try
        {
            var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var permission = await _rolePermissionService.GetPermissionByRole(roleName);
            HttpContext.Session.SetString("permission",JsonConvert.SerializeObject(permission));
            
            TableViewOrderAppViewModel model = await _tableOrderAppService.GetTableView();
            return View(model);
        }
        catch (System.Exception e)
        {
            //   return error Page
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> AssignTable(int? id)
    {
        try
        {
            AddEditWaitingTokenViewModel model = await _tableOrderAppService.AssignTableOffcanvasData(id);
            return PartialView("_AssignTableOffcanvas", model);
        }
        catch (System.Exception e)
        {
            AddEditWaitingTokenViewModel model = new AddEditWaitingTokenViewModel();
            return PartialView("_AssignTableOffcanvas", model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AssignTable([FromForm] AddEditWaitingTokenViewModel model, string TableIds)
    {

        if (!ModelState.IsValid)
            return Json(new { isValid = false, message = "Please fill all required fields" });
        var authToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(authToken);
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            return Json(new { isValid = false, message = "Your session is expired" });

        try
        {
            var tableIds = JsonConvert.DeserializeObject<List<int>>(TableIds);

            int orderId = await _tableOrderAppService.AssignTable(model, userId, tableIds);
            if(orderId == -1)
                return Json(new { isSuccess = false, message = "This customer has a running order.", orderId = orderId });
            if (orderId != null)
                return Json(new { isSuccess = true, message = "Table Assigned Successfully", orderId = orderId });
            return Json(new { isSuccess = false, message = "Error while assign table" });
        }
        catch (System.Exception e)
        {
            return Json(new { isSuccess = false, message = "Error while assign table" });
        }
    }

}
