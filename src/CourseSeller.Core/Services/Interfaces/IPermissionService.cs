using CourseSeller.DataLayer.Entities.Permissions;

namespace CourseSeller.Core.Services.Interfaces;

public interface IPermissionService
{
    Task<List<Permission>> GetAll();
    Task AddPermissionsToRole(int roleId, List<int> permissions);
    Task<List<int>> GetRoleSelectedPermissionsId(int roleId);
    Task UpdateRolePermission(int roleId, List<int> permissions);
    Task<bool> UserHasPermission(int permissionId, string userName);
}