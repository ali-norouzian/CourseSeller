using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services
{
    public class RoleService : IRoleService
    {
        private readonly MssqlContext _context;

        public RoleService(MssqlContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<int> AddRole(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return role.RoleID;

        }

        public async Task<Role> GetRoleById(int roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRole(Role role)
        {
            role.IsDelete = true;
            await UpdateRole(role);
        }
    }
}
