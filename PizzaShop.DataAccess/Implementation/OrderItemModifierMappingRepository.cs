using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class OrderItemModifierMappingRepository:IOrderItemModifierMappingRepository
{
    public readonly PizzashopContext _context;

    public OrderItemModifierMappingRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task AddNewOrderItemModifierMapping(Orderitemmodifiermapping orderItemModifierMapping)
    {
        await _context.Orderitemmodifiermappings.AddAsync(orderItemModifierMapping);
        await _context.SaveChangesAsync();
    }

    public async Task AddMapping(Orderitemmodifiermapping orderItemModifierMapping)
    {
        await _context.Orderitemmodifiermappings.AddAsync(orderItemModifierMapping);
        await _context.SaveChangesAsync();
    }

}
