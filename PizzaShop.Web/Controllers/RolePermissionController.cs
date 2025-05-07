using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.ViewModels;
using PizzaShop.Web.Authorization;

namespace PizzaShop.Web.Controllers;

[PermissionAuthorize("Role.View")]
public class RolePermissionController : Controller
{
    private readonly IUsersService _user;
    private readonly IRolePermissionService _rolePermission;
    private readonly IJWTService _jwtService;
    private readonly IRolePermissionService _rolePermissionService;
    public RolePermissionController(IUsersService user, IRolePermissionService rolePermission, IRoleRepository role, IJWTService jwtService, IRolePermissionService rolePermissionService)
    {
        _user = user;
        _rolePermission = rolePermission;
        _jwtService = jwtService;
        _rolePermissionService = rolePermissionService;
    }

    [PermissionAuthorize("Role.View")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";

        var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var permission = await _rolePermissionService.GetPermissionByRole(roleName);
        HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));

        var authToken = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(authToken))
        {
            return RedirectToAction("Login", "Authentication");
        }
        var (email, role, userId) = _jwtService.ValidateToken(authToken);
        // if (role != "Admin")
        //     return RedirectToAction("Dashboard", "Dashboard");

        List<Role> roles = await _rolePermission.GetAllRoles();
        return View(roles);
    }

    [PermissionAuthorize("Role.View")]
    [HttpGet]
    public async Task<IActionResult> ManageRolePermission(int id)
    {
        var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var permission = await _rolePermissionService.GetPermissionByRole(roleName);
        HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));
        
        var AuthToken = Request.Cookies["AuthToken"];
        if (String.IsNullOrEmpty(AuthToken))
        {
            return RedirectToAction("Login", "Authentication");
        }
        var model = await _rolePermission.GetRolePermissionByRoleId(id);
        if (model != null)
            return View(model);
        return RedirectToAction("Index", "RolePermission");
    }

    [PermissionAuthorize("Role.AddEdit")]
    [HttpPost]
    public async Task<IActionResult> ManageRolePermission(List<ManageRolePermissioinViewModel> model)
    {
        var AuthToken = Request.Cookies["AuthToken"];
        if (String.IsNullOrEmpty(AuthToken))
        {
            return RedirectToAction("Login", "Authentication");
        }
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        var permission = await _rolePermission.GetPermissionByRole(role);
        HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));

        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login", "Authentication");

        bool updateRolePermission = await _rolePermission.UpdateRolePermission(model, userId);
        if (updateRolePermission)
        {
            TempData["SuccessUpdate"] = "Role and permissions updated successfully";
            var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var permissions = await _rolePermissionService.GetPermissionByRole(roleName);
            HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permissions));
            return Redirect("/RolePermission/ManageRolePermission?id="+model.FirstOrDefault().RoleId);
        }

        TempData["ErrorUpdate"] = "Something went wrong pls try again";
        return View(model);
    }
}