using PizzaShop.DataAccess.Data;

namespace PizzaShop.DataAccess.Interfaces;

public interface IRoleRepository
{
    public Task<string> GetRoleById(int? id);
    public Task<List<Role>> GetAllRoles();
}
