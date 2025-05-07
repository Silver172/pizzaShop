using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;

namespace PizzaShop.DataAccess.Implementation;

public class UnitRepository: IUnitRepository
{
    private readonly PizzashopContext _context;

    public UnitRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<Unit>> GetUnits()
    {
        var Units = await _context.Units.ToListAsync();
        return Units;
    }

    public async Task<Unit?> GetUnitById(int? id)
    {
        return await _context.Units.FirstOrDefaultAsync(u => u.Id == id);
    }
}
