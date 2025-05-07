using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Authorization;

namespace PizzaShop.Web.Controllers;

[Authorize(Roles = "Admin,Account Manager")]
public class DashboardController: Controller
{
    private readonly IJWTService _jwtService;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly IDashboardService _dashboardService;
    public DashboardController(IJWTService jwtService,IRolePermissionService rolePermissionService,IDashboardService dashboardService)
    {
        _jwtService = jwtService;
        _rolePermissionService = rolePermissionService;
        _dashboardService = dashboardService;
    }
    [Authorize]
    [PermissionAuthorize("")]
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";

        var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var permission = await _rolePermissionService.GetPermissionByRole(roleName);
        HttpContext.Session.SetString("permission",JsonConvert.SerializeObject(permission));
        var AuthToken = Request.Cookies["AuthToken"];
        
        if (string.IsNullOrEmpty(AuthToken))
            return RedirectToAction("Login", "Authentication");
        
        var (userEmail,role,userId) = _jwtService.ValidateToken(AuthToken);
        if(userEmail != null && role != null)
        {
            DashboardViewModel dashboard = await _dashboardService.GetDashboardPage("Current Month");
            return View(dashboard);
            // return View();
        }
        
        TempData["ToastrMessage"] = "Your session is expired !";
        TempData["ToastrType"] = "warning";
        
        return RedirectToAction("Login","Authentication");
    }

    public async Task<IActionResult> GetDashboardData(string? TimePeriod)
    {

        DashboardViewModel dashboard = new DashboardViewModel();
        try
        {
            dashboard = await _dashboardService.GetDashboardPage(TimePeriod);
            return PartialView("_DashboardData",dashboard);
        }
        catch (System.Exception)
        {
            return PartialView("_DashboardData",dashboard);
        }
    }
}
