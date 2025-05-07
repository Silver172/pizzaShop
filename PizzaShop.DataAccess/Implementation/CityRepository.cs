using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;

namespace PizzaShop.DataAccess.Implementation;

public class CityRepository: ICityRepository
{
    private readonly PizzashopContext _context;

    public CityRepository(PizzashopContext context)
    {
        _context = context;
    }
    
    public async Task<List<City>> GetAllCities()
    {
        var allCities = await _context.Cities.ToListAsync();
        return allCities;
    }

    public async Task<List<City>> GetCitiesByStateId(int id)
    {
        var cities = await _context.Cities.Where(c => c.Stateid == id).ToListAsync();
        return cities;
    }
}
