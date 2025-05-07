using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.Interfaces;

public interface ICountryRepository
{
    public Task<List<Country>> GetAllCountry();
}
