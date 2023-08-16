using CourseSeller.Core.DTOs.UserPanel;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class UserPanelService : IUserPanelService
{
    private MssqlContext _context;
    private IAccountService _accountService;

    public UserPanelService(MssqlContext context, IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task<UserInfoViewModel> GetUserInfo(string userName)
    {
        var query = _context.Users.Where(u => u.UserName == userName)
            .Select(u => new UserInfoViewModel()
            {
                UserName = u.UserName,
                ImageName = u.UserAvatar,
                RegisterDateTime = u.RegisterDateTime,
                Email = u.Email,
                Wallet = 0,
            });

        return await query.SingleOrDefaultAsync();
    }

}