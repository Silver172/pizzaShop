using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Authorization;

namespace PizzaShop.Web.Controllers;

[PermissionAuthorize("Customers.View")]
public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;
     private readonly IRolePermissionService _rolePermissionService;
    public CustomerController(ICustomerService customerService,IRolePermissionService rolePermissionService)
    {
        _customerService = customerService;
        _rolePermissionService = rolePermissionService;
    }

    [PermissionAuthorize("Customers.View")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var permission = await _rolePermissionService.GetPermissionByRole(roleName);
        HttpContext.Session.SetString("permission",JsonConvert.SerializeObject(permission));

        CustomerPageViewModel customerPage = await _customerService.GetCustomerPage();
        return View(customerPage);
    }

    [PermissionAuthorize("Customers.View")]
    [HttpGet]
    public async Task<IActionResult> GetFilteredCustomers(int pageIndex, int pageSize, string? searchString, string? time, string? fromDate, string? toDate, string? sortBy = "Name", string? sortType = "ascending")
    {
        var pagination = new PaginationViewModel
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchString = searchString,
            FromDate = string.IsNullOrEmpty(fromDate) ? null : DateTime.Parse(fromDate),
            ToDate = string.IsNullOrEmpty(toDate) ? null : DateTime.Parse(toDate),
            Time = time,
            SortingBy = sortBy,
            SortingType = sortType
        };
        CustomerPageViewModel customerPage = await _customerService.GetFilteredCustomer(pagination);
        return PartialView("_CustomerList", customerPage);
    }

    [HttpGet]
    public async Task<IActionResult> ExportToExcel(int pageIndex, int pageSize, string? searchString, string? fromDate, string? toDate, string? status, string? time, string? sortBy = "Id", string? sortType = "ascending")
    {
        var pagination = new PaginationViewModel
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SearchString = searchString,
            FromDate = string.IsNullOrEmpty(fromDate) ? null : DateTime.Parse(fromDate),
            ToDate = string.IsNullOrEmpty(toDate) ? null : DateTime.Parse(toDate),
            Time = time,
            Status = status,
            SortingBy = sortBy,
            SortingType = sortType
        };
    return await _customerService.ExportCustomersExcel(pagination);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerHistory(int id)
    {
        CustomerHistoryViewModel model = await _customerService.GetCustomerHistory(id);
        return PartialView("_CustomerHistory", model);
    }

}
