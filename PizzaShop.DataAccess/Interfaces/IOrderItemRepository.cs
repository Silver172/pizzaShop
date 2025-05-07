using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IOrderItemRepository
{
    public Task<bool> MarkOrderItemStatus(OrderItemViewModel data, string userId, string filterBy);
    public Task<List<Orderitem>> GetOrderItemsByOrderId(int orderId);
    public Task AddNewOrderItem(Orderitem orderitem);
    public Task UpdateOrderItem(OrderItemViewModel orderitem, string userId);
    public Task DeleteOrderItem(int id, string userId);
    public Task<Orderitem> GetOrderItemById(int id);
    public Task<bool> SaveComment(int id, string comment, string? userId);
}
