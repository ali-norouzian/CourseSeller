using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.Core.Services.Interfaces;

public interface IRoleService
{
    Task<List<Role>> GetAllRoles();
    Task<int> AddRole(Role role);
    Task<Role> GetRoleById(int roleId);
    Task UpdateRole(Role role);
    Task DeleteRole(Role role);
}