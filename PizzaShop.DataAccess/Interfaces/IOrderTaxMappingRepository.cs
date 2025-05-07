using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IOrderTaxMappingRepository
{
    public Task AddNewOrderTaxMapping(Ordertaxmapping orderTaxMapping);
    public Task UpdateOrderTaxMapping(OrderTaxViewModel orderTax,string userId);
}
