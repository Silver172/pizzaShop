using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IOrderRepository
{
    public Task<List<Order>> GetOrderList(PaginationViewModel pagination);
    public Task<List<Order>> GetOrdersCount(PaginationViewModel pagination);
    public Task<Order> OrderDetails(int id);
    public Task<List<Order>> GetKotContent(int categoryId, PaginationViewModel pagination);
    public Task<int> GetKotContentCount(int categoryId, PaginationViewModel pagination);
    public Task<Order> creteOrder(AddEditWaitingTokenViewModel model, string userId);
    public Task<bool> UpdateOrder(OrderDetailViewModel order, string userId);
    public Task<bool> AddEditOrderComment(int id, string comments, string userId);
    public Task UpdateOrderStatus(int id, string status, string userId);
    public Task<List<Order>> GetOrdersByCustomerId(int id);
    public Task<DashboardViewModel> GetDashboardData(string? TimePeriod = "Current Month");
}
