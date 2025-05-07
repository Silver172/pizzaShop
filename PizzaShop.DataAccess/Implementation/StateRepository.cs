using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;

namespace PizzaShop.DataAccess.Implementation;

public class StateRepository: IStateRepository
{
    private readonly PizzashopContext _context;

    public StateRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<State>> GetAllStates()
    {
        var allStates = await _context.States.ToListAsync();
        return allStates;
    }

    public async Task<List<State>> GetStatesByCountryId(int id)
    {
        var states = await _context.States.Where(s => s.Countryid == id).ToListAsync();
        return states;
    }
}
