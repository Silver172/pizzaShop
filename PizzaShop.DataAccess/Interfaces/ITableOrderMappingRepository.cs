using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface ITableOrderMappingRepository
{
    public Task AddMapping(AddEditWaitingTokenViewModel model,int tableId,int orderId,string userId);
    public Task<List<Tableordermapping>> GetTableOrderMappingByOrderId(int id);
}
