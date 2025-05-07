using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.Interfaces;

public interface IStateRepository
{
    public Task<List<State>> GetAllStates();

    public Task<List<State>> GetStatesByCountryId(int id);
}
