using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IKOTOrderAppService
{
    public Task<OrderAppKOTViewModel> GetKotData(int categoryId);
    public Task<OrderAppKOTViewModel> GetKOTByCategoryId(int categoryId,int pageIndex,string? filterBy);
    public Task<bool> MarkOrderItemStatus(List<OrderItemViewModel> data,string userId, string filterBy);
}
