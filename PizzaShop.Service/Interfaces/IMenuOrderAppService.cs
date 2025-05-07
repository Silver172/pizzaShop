using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IMenuOrderAppService
{
    public Task<MenuOrderAppViewModel> GetMenuItemsByCategoryId(int cId, string? searchString);
    public Task<List<ItemModifierViewModel>> GetModifiersByItemId(int itemId);
    public Task<MenuOrderAppViewModel> GetMenuWithOrderDetail(int? orderId);
    public Task<bool> MarkAsFavourite(int itemId);
    public Task<bool> SaveOrder(OrderDetailViewModel order, string userId);
    public Task<bool> CheckIsItemReady(int id);
    public Task<bool> AddEditOrderComments(int id, string comment, string userId);
    public Task<int> UpdateCustomer(int id, string name, string email, string phone, int noOfPerson, string? userId);
    public Task<bool> SaveOrderItemComment(int id, string comment, string? userId);
    public Task<bool> CanReduceItemQuantity(int id, int quantity);
    public Task<int> CompleteOrder(int id, string userId);
    public Task<int> CancelOrder(int orderId, string userId);
    public Task<bool> SaveCustomerFeedBack(int orderId, short? food, short? service, short? ambience, string? comment, string? userId);
}
