using System.Threading.Tasks;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementation;

public class AddressService : IAddressService
{
    private readonly ICountryRepository _country;
    private readonly IStateRepository _state;
    private readonly ICityRepository _city;

    public AddressService(ICountryRepository country, IStateRepository state, ICityRepository city)
    {
        _country = country;
        _city = city;
        _state = state;
    }

    public async Task<List<State>> GetAllStates(int id)
    {
        List<State> states;
        if (id == -1)
        { states = await _state.GetAllStates(); }
        else
        { states = await _state.GetStatesByCountryId(id); }
        return states;
    }

    public async Task<List<City>> GetAllCities(int id)
    {
        List<City> cities;
        if (id == -1)
        { cities = await _city.GetAllCities(); }
        else
        { cities = await _city.GetCitiesByStateId(id); }
        return cities;
    }

    public async Task<List<Country>> GetAllCountries()
    {
        var countries = await _country.GetAllCountry();
        return countries;
    }

}
