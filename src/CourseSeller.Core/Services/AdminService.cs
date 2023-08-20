using CourseSeller.Core.DTOs.Admin;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class AdminService : IAdminService
{
    private readonly MssqlContext _context;

    public AdminService(MssqlContext context)
    {
        _context = context;
    }

    public async Task<UsersViewModel> GetAllUsers(int pageId = 1, string filterEmail = "", string filterUserName = "")
    {
        IQueryable<User> result = _context.Users;

        if (!string.IsNullOrEmpty(filterEmail))
            result = result.Where(u => u.Email.Contains(filterEmail));
        if (!string.IsNullOrEmpty(filterUserName))
            result = result.Where(u => u.UserName.Contains(filterUserName));

        // Show item in each page
        int take = 10;
        int skip = (pageId - 1) * take;

        UsersViewModel list = new()
        {
            Users = await result.OrderBy(u => u.RegisterDateTime)
                .Skip(skip).Take(take).ToListAsync(),
            CurrentPage = pageId,
            PageCount = await result.CountAsync() / take
        };

        return list;
    }
}

