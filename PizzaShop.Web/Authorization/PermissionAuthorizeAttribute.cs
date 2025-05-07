namespace PizzaShop.Web.Authorization;

using Microsoft.AspNetCore.Authorization;

public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    public PermissionAuthorizeAttribute(string permission) : base()
    {
        Policy = $"{permission}";
    }
}

