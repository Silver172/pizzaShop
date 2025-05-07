using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class TableOrderMappingRepository: ITableOrderMappingRepository
{
    private readonly PizzashopContext _context;

    public TableOrderMappingRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task AddMapping(AddEditWaitingTokenViewModel model,int tableId,int orderId,string userId)
    {
        var tableOrderMapping = new Tableordermapping
        {
            Tableid = tableId,
            Orderid = orderId,
            Noofpersons = model.NoOfPersons,
            Createdby = userId,
            Createddate = DateTime.Now,
            Isdeleted = false
        };
        await _context.Tableordermappings.AddAsync(tableOrderMapping);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Tableordermapping>> GetTableOrderMappingByOrderId(int id)
    {
        var mapping = await _context.Tableordermappings.Where(t => t.Orderid == id && t.Isdeleted == false)
        .Include(t => t.Table)
        .ToListAsync();
        return mapping;
    }
}
