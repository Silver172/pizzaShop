using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.Interfaces;

public interface IUnitRepository
{
    public Task<List<Unit>> GetUnits();
    public Task<Unit?> GetUnitById(int? id);
}
