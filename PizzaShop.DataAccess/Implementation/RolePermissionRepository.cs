using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.ViewModels;

namespace PizzaShop.DataAccess.Implementation;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly PizzashopContext _context;
    private readonly IRoleRepository _role;
    public RolePermissionRepository(PizzashopContext context, IRoleRepository role)
    {
        _context = context;
        _role = role;
    }

    public async Task<List<Permission>?> GetAllPermissions()
    {
        List<Permission> permissions = await _context.Permissions.ToListAsync();
        if (permissions.Count() > 0)
            return permissions;
        return null;
    }

    public async Task<List<ManageRolePermissioinViewModel>?> GetRolePermissionByRoleId(int roleId)
    {
        List<Rolepermission> rolePermission = await _context.Rolepermissions.Where(rp => rp.Roleid == roleId).OrderBy(rp => rp.Permissionid).ToListAsync();
        if (rolePermission.Count() > 1)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
            var rolePermissionList = new List<ManageRolePermissioinViewModel>();

            foreach (var rp in rolePermission)
            {
                rolePermissionList.Add(new ManageRolePermissioinViewModel
                {
                    Id = rp.Id,
                    RoleId = rp.Roleid,
                    RoleName = role.Name,
                    Permissionid = rp.Permissionid,
                    PermissionName = _context.Permissions.FirstOrDefault(p => p.Id == rp.Permissionid)?.Name,
                    Canaddedit = rp.Canaddedit == true ? true : false,
                    Canview = rp.Canview == true ? true : false,
                    Candelete = rp.Candelete == true ? true : false
                });
            }

            return rolePermissionList;
        }

        return null;
    }

    public async Task<bool> UpdateRolePermission(List<ManageRolePermissioinViewModel> model, string userId)
    {
        foreach (var rp in model)
        {
            if (rp.Permissionid != 2)
            {
                var rolePermission = _context.Rolepermissions.FirstOrDefault(x => x.Id == rp.Id);
                rolePermission.Canaddedit = rp.Canaddedit;
                rolePermission.Canview = rp.Canview;
                rolePermission.Candelete = rp.Candelete;
                rolePermission.Updateddate = DateTime.Now;
                rolePermission.Updatedby = userId;
                await _context.SaveChangesAsync();
            }
        }
        return true;
    }

    public async Task<List<ManageRolePermissioinViewModel>?> GetPermissionsByRole(string role)
    {
        var roleData = await _context.Roles.FirstOrDefaultAsync(r => r.Name.Trim() == role);
        if (role == null)
            return null;
        return await GetRolePermissionByRoleId(roleData.Id);
    }

}
