using PizzaShop.DataAccess.Data;

namespace PizzaShop.Service.Interfaces;

public interface IAddressService
{
    public Task<List<State>> GetAllStates(int id);
    public Task<List<City>> GetAllCities(int id);
    public Task<List<Country>> GetAllCountries();
}
