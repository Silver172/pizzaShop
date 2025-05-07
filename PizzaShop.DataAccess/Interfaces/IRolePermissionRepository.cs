using PizzaShop.DataAccess.Data;
using PizzaShop.ViewModels;

namespace PizzaShop.DataAccess.Interfaces;

public interface IRolePermissionRepository
{
    public Task<List<Permission>?> GetAllPermissions();
    public Task<List<ManageRolePermissioinViewModel>?> GetRolePermissionByRoleId(int roleId);

    public Task<bool> UpdateRolePermission(List<ManageRolePermissioinViewModel> model,string userId);
    public Task<List<ManageRolePermissioinViewModel>?> GetPermissionsByRole(string role);
}
