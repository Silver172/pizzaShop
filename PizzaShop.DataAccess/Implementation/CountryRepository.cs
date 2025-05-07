using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;

namespace PizzaShop.DataAccess.Implementation;

public class CountryRepository : ICountryRepository
{
    private readonly PizzashopContext _context;

    public CountryRepository(PizzashopContext context)
    {
        _context = context;
    }
    public async Task<List<Country>> GetAllCountry()
    {
        var allCountries = await _context.Countries.ToListAsync();
        return allCountries;
    }
}
