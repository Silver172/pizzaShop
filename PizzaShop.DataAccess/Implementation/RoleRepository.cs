using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;

namespace PizzaShop.DataAccess.Implementation;

public class RoleRepository: IRoleRepository
{
    private readonly PizzashopContext _context;

    public RoleRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<string> GetRoleById(int? id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        return role.Name;
    }

    public async Task<List<Role>> GetAllRoles()
    {

        var roles = await _context.Roles.OrderBy(r => r.Id).ToListAsync();
        var rollList = new List<Role>();
        foreach(var role in roles)
        {
            rollList.Add(new Role
            {
                Id = role.Id,
                Name = role.Name
            });
        };
        return roles;
    }
}
