using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Authorization;

namespace PizzaShop.Web.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRolePermissionService _rolePermissionService;
    private readonly IJWTService _jWTService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionHandler(IRolePermissionService rolePermissionService, IJWTService jWTService, IHttpContextAccessor httpContextAccessor)
    {
        this._rolePermissionService = rolePermissionService;
        this._jWTService = jWTService;
        this._httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        bool isEditError = false;
        var cookieSavedToken = httpContext.Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(cookieSavedToken))
        {
            context.Fail();
            return;
        }
        var (email, role, userId) = _jWTService.ValidateToken(cookieSavedToken);
        var permissionsData = await _rolePermissionService.GetPermissionByRole(role);

        // return status code 400

        switch (requirement.Permission)
        {
            case "Users.View":
                if (permissionsData[0].Canview == true)
                    context.Succeed(requirement);
                break;
            case "Users.AddEdit":
                if (permissionsData[0].Canaddedit == true)
                    context.Succeed(requirement);
                break;
            case "Users.Delete":
                if (permissionsData[0].Candelete == true)
                    context.Succeed(requirement);
                break;
            case "Role.View":
                if (permissionsData[1].Canview == true)
                    context.Succeed(requirement);
                break;
            case "Role.AddEdit":
                if (permissionsData[1].Canaddedit == true)
                    context.Succeed(requirement);
                break;
            case "Role.Delete":
                if (permissionsData[1].Candelete == true)
                    context.Succeed(requirement);
                break;
            case "Menu.View":
                if (permissionsData[2].Canview == true)
                    context.Succeed(requirement);
                break;
            case "Menu.AddEdit":
                if (permissionsData[2].Canview && permissionsData[2].Canaddedit)
                    context.Succeed(requirement);
                else
                    isEditError = true;
                break;
            case "Menu.Delete":
                if (permissionsData[2].Canview && permissionsData[2].Candelete == true)
                    context.Succeed(requirement);
                else
                    isEditError = true;
                break;
            case "TableSection.View":
                if (permissionsData[3].Canview == true)
                    context.Succeed(requirement);
                break;
            case "TableSection.AddEdit":
                if (permissionsData[3].Canaddedit == true)
                    context.Succeed(requirement);
                break;
            case "TableSection.Delete":
                if (permissionsData[3].Candelete == true)
                    context.Succeed(requirement);
                break;
            case "TaxFees.View":
                if (permissionsData[4].Canview == true)
                    context.Succeed(requirement);
                break;
            case "TaxFees.AddEdit":
                if (permissionsData[4].Canaddedit == true)
                    context.Succeed(requirement);
                break;
            case "TaxFees.Delete":
                if (permissionsData[4].Candelete == true)
                    context.Succeed(requirement);
                break;
            case "Orders.View":
                if (permissionsData[5].Canview == true)
                    context.Succeed(requirement);
                break;
            case "Orders.AddEdit":
                if (permissionsData[5].Canaddedit == true)
                    context.Succeed(requirement);
                break;
            case "Orders.Delete":
                if (permissionsData[5].Candelete == true)
                    context.Succeed(requirement);
                break;
            case "Customers.View":
                if (permissionsData[6].Canview == true)
                    context.Succeed(requirement);
                break;
            case "Customers.AddEdit":
                if (permissionsData[6].Canaddedit == true)
                    context.Succeed(requirement);
                break;
            case "Customers.Delete":
                if (permissionsData[6].Candelete == true)
                    context.Succeed(requirement);
                break;
            default:
                break;
        }

        if (isEditError)
        {
            context.Fail();
            httpContext.Response.StatusCode = 400;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync("You do not have permission to perform this action.");
            return;
        }

        return;
    }

}