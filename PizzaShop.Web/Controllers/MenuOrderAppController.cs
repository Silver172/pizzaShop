using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.DataAccess.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize(Roles = "Admin,Account Manager")]
public class MenuOrderAppController : Controller
{
  private readonly IMenuOrderAppService _menuOrderAppService;
  private readonly IJWTService _jwtService;
  private readonly IRolePermissionService _rolePermissionService;
  public MenuOrderAppController(IMenuOrderAppService menuOrderAppService, IJWTService jWTService,IRolePermissionService rolePermissionService)
  {
    _menuOrderAppService = menuOrderAppService;
    _jwtService = jWTService;
    _rolePermissionService = rolePermissionService;
  }

  [HttpGet]
  public async Task<IActionResult> Index(int? id)
  {
    try
    {
      var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
      var permission = await _rolePermissionService.GetPermissionByRole(roleName);
      HttpContext.Session.SetString("permission",JsonConvert.SerializeObject(permission));

      MenuOrderAppViewModel model = new MenuOrderAppViewModel();
      if (id != null)
      {
        model = await _menuOrderAppService.GetMenuWithOrderDetail(id);
      }
      else
      {
        model = await _menuOrderAppService.GetMenuItemsByCategoryId(0, "");
      }
      return View(model);
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Something went wrong" });
    }
  }


  [HttpGet]
  public async Task<IActionResult> GetItemsByCategoryId(int cId, string? searchString)
  {
    try
    {
      MenuOrderAppViewModel model = await _menuOrderAppService.GetMenuItemsByCategoryId(cId, searchString);
      return PartialView("_MenuItems", model);
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Something went wrong" });
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetItemModifier(int itemId)
  {
    try
    {
      var model = await _menuOrderAppService.GetModifiersByItemId(itemId);
      return PartialView("_ItemModifierModal", model);
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Something went wrong" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> MarkAsFavourite(int itemId)
  {
    try
    {
      var result = await _menuOrderAppService.MarkAsFavourite(itemId);
      if (!result)
      {
        return Json(new { success = false, message = "Item not found" });
      }
      return Json(new { success = true, message = "Item marked as favourite" });
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Something went wrong" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> SaveOrder(OrderDetailViewModel order)
  {
    try
    {
      var authToken = Request.Cookies["AuthToken"];
      var (userEmail, role, userId) = _jwtService.ValidateToken(authToken);
      bool isOrderPlaced = await _menuOrderAppService.SaveOrder(order, userId);
      if (isOrderPlaced)
        return Json(new { success = true, message = "Order saved successfully" });
      return Json(new { success = false, message = "Error while save Order" });
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Error while save an order" });
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetOrderDetails(int id)
  {
    try
    {
      MenuOrderAppViewModel model = await _menuOrderAppService.GetMenuWithOrderDetail(id);
      return PartialView("_OrderItemDetail", model.OrderDetails);
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Error while fetching data" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> CheckIsItemReady(int id)
  {
    try
    {
      var authToken = Request.Cookies["AuthToken"];
      var (userEmail, role, userId) = _jwtService.ValidateToken(authToken);
      bool isItemReady = await _menuOrderAppService.CheckIsItemReady(id);
      if (isItemReady)
        return Json(new { success = true, message = "Item is prepared." });
      return Json(new { success = false, message = "Item is not prepared" });
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Error while checking item status" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> SaveOrderComment(int id, string comment)
  {
    var authToken = Request.Cookies["AuthToken"];
    var (userEmail, role, userId) = _jwtService.ValidateToken(authToken);
    try
    {
      var isUpdated = await _menuOrderAppService.AddEditOrderComments(id, comment, userId);
      if (isUpdated)
        return Json(new { success = true, message = "Comment added successfully" });
      else
        return Json(new { success = false, message = "Please write the comment." });
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Error while adding comment" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> UpdateCustomer(int id, string name, string email, string phone, int noOfPerson)
  {
    var authToken = Request.Cookies["AuthToken"];
    var (userEmail, role, userId) = _jwtService.ValidateToken(authToken);
    try
    {
      int response = await _menuOrderAppService.UpdateCustomer(id, name, email, phone, noOfPerson, userId);
      if (response == 1)
        return Json(new { success = true, message = "Customer updated successfully." });
      else
        return Json(new { success = false, message = "Exceed table capacity." });
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Error while update customer" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> SaveOrderItemComment(int id, string comment)
  {
    var authToken = Request.Cookies["AuthToken"];
    var (userEmail, role, userId) = _jwtService.ValidateToken(authToken);
    try
    {
      var isUpdated = await _menuOrderAppService.SaveOrderItemComment(id, comment, userId);
      if (isUpdated)
        return Json(new { success = true, message = "Comment added successfully" });
      else
        return Json(new { success = false, message = "Please write the comment." });
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Error while adding comment" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> CanReduceItemQuantity(int id, int quantity)
  {
    try
    {
      var authToken = Request.Cookies["AuthToken"];
      var (userEmail, role, userId) = _jwtService.ValidateToken(authToken);
      bool canReduce = await _menuOrderAppService.CanReduceItemQuantity(id, quantity);
      if (canReduce)
        return Json(new { success = true, message = "Can Reduce quantity." });
      return Json(new { success = false, message = "Item is prepared" });
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Error while checking item status" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> CompleteOrder(int id)
  {
    var authToken = Request.Cookies["AuthToken"];
    var (email, role, userId) = _jwtService.ValidateToken(authToken);
    try
    {
      int response = await _menuOrderAppService.CompleteOrder(id, userId);
      if (response == 0)
        return Json(new { success = false, message = "All items must be served before completing the order" });
      else
        return Json(new { success = true, message = "Order completed successfully" });
    }
    catch (System.Exception)
    {
      return Json(new { success = false, message = "Error while completing the order" });
    }
  }

  [HttpPost]
  public async Task<IActionResult> CancelOrder(int id)
  {
    var authToken = Request.Cookies["AuthToken"];
    var (email, role, userId) = _jwtService.ValidateToken(authToken);
    var OrderCanceled = await _menuOrderAppService.CancelOrder(id, userId);
    if (OrderCanceled == -1)
    {
      return Json(new { success = false, message = "Some Error Occurred while Cancel Order." });
    }
    else if (OrderCanceled == 0)
    {
      return Json(new { success = false, message = "The order item is ready, cannot cancel the order." });
    }

    return Json(new { success = true, message = "Order Canceled Successfully." });
  }

  [HttpPost]
  public async Task<IActionResult> SaveCustomerFeedBack(int orderId, short? food, short? service, short? ambience, string? comment)
  {
    var authToken = Request.Cookies["AuthToken"];
    var (email, role, userId) = _jwtService.ValidateToken(authToken);
    bool response = await _menuOrderAppService.SaveCustomerFeedBack(orderId, food, service, ambience, comment, userId);
    if (response)
      return Json(new { success = true, message = "Thank you for your review!" });
    else
      return Json(new { success = false, message = "Error while add review." });
  }
}
