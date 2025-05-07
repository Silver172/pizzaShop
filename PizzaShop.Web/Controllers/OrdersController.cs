using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Authorization;

namespace PizzaShop.Web.Controllers;

[PermissionAuthorize("Orders.View")]
[Authorize]
public class OrdersController : Controller
{
    private readonly IJWTService _jwtService;
    private readonly IOrderService _orderService;
    private readonly IWebHostEnvironment _webHost;
    private readonly IRolePermissionService _rolePermissionService;
    public OrdersController(IJWTService jwtService, IOrderService orderService, IWebHostEnvironment webHost,IRolePermissionService rolePermissionService)
    {
        _jwtService = jwtService;
        _orderService = orderService;
        _webHost = webHost;
        _rolePermissionService = rolePermissionService;
    }

    [PermissionAuthorize("Orders.View")]
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
        var pagination = new PaginationViewModel
        {
            PageIndex = 1,
            PageSize = 5,
            SearchString = "",
            FromDate = null,
            Time = "All Time",
            ToDate = null,
            Status = "All Status",
            SortingBy = "Id",
            SortingType = "ascending"
        };
        OrdersPageViewModel orderPage = await _orderService.GetOrders(pagination);

        return View(orderPage);
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredOrders(int pageIndex, int pageSize, string? searchString, string? fromDate, string? toDate, string? status, string? time, string? sortBy = "Id", string? sortType = "ascending")
    {
        var AuthToken = Request.Cookies["AuthToken"];
        var (email, role, userId) = _jwtService.ValidateToken(AuthToken);
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Authentication", "Login");

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
        OrdersPageViewModel OrderPage = await _orderService.GetOrders(pagination);
        return PartialView("_OrdersList", OrderPage);
    }

    [HttpPost]
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
        return await _orderService.ExportOrderExcel(pagination);
    }

    [PermissionAuthorize("Orders.View")]
    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {

        OrderDetailViewModel orderDetail = await _orderService.OrderDetails(id);
        return View(orderDetail);
    }

    [PermissionAuthorize("Orders.View")]
    public async Task<IActionResult> InvoiceDetail(int id)
    {
        OrderDetailViewModel orderDetail = await _orderService.OrderDetails(id);
        return PartialView("_InvoiceDetail",orderDetail);
    }
}
