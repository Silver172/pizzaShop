using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.Interfaces;

public interface IOrderItemModifierMappingRepository
{
    public Task AddMapping(Orderitemmodifiermapping orderItemModifierMapping);
}
