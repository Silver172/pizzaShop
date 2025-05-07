using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class OrderTaxMappingRepository:IOrderTaxMappingRepository
{
    public readonly PizzashopContext _context;

    public OrderTaxMappingRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task AddNewOrderTaxMapping(Ordertaxmapping orderTaxMapping)
    {
        await _context.Ordertaxmappings.AddAsync(orderTaxMapping);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrderTaxMapping(OrderTaxViewModel orderTax,string userId)
    {
        Ordertaxmapping updatedOrderTax = await _context.Ordertaxmappings.FirstOrDefaultAsync(i => i.Id == orderTax.Id && i.Isdeleted == false);
        
        updatedOrderTax.Taxvalue = (decimal)orderTax.Taxvalue;
        updatedOrderTax.Updatedby = userId;
        updatedOrderTax.Updateddate = DateTime.Now;
        await _context.SaveChangesAsync();
    }
}

