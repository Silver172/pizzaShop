using PizzaShop.DataAccess.Data;
using PizzaShop.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IRolePermissionService
{
    public Task<List<Role>> GetAllRoles();
    public Task<List<ManageRolePermissioinViewModel>?> GetRolePermissionByRoleId(int roleId);
    public Task<bool> UpdateRolePermission(List<ManageRolePermissioinViewModel> model,string userId);
    public Task<List<ManageRolePermissioinViewModel>?> GetPermissionByRole(string role);
}
