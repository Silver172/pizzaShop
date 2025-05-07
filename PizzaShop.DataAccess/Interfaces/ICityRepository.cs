using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.Interfaces;

public interface ICityRepository
{
    public Task<List<City>> GetAllCities();
    public Task<List<City>> GetCitiesByStateId(int id);
}
