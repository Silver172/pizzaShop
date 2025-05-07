using System.Threading.Tasks;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.ViewModels;

namespace PizzaShop.Service.Implementation;

public class RolePermissionService: IRolePermissionService
{
    private readonly IAccountRepository _account;
    private readonly IRoleRepository _role;
    private readonly IRolePermissionRepository _rolePermission;
    

    public RolePermissionService(IAccountRepository account,IRoleRepository role,IRolePermissionRepository rolePermission)
    {
        _account = account;
        _role = role;
        _rolePermission = rolePermission;
    }

    public async Task<List<Role>> GetAllRoles()
    {
        return await _role.GetAllRoles();
    }

    public async Task<List<ManageRolePermissioinViewModel>?> GetRolePermissionByRoleId(int roleId)
    {
        var rolePermission = await _rolePermission.GetRolePermissionByRoleId(roleId);
        if(rolePermission.Count() > 0)
            return rolePermission;
        return null;
    }
    
    public async Task<bool> UpdateRolePermission(List<ManageRolePermissioinViewModel> model,string userId)
    {
        bool isRolePermission = await _rolePermission.UpdateRolePermission(model,userId);
        if(isRolePermission)
            return true;
        return false;
    }

    public async Task<List<ManageRolePermissioinViewModel>?> GetPermissionByRole(string role)
    {
        return await _rolePermission.GetPermissionsByRole(role);
    }
}
