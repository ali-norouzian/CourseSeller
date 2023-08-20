using CourseSeller.Core.Convertors;
using CourseSeller.Core.DTOs.Admin;
using CourseSeller.Core.Generators;
using CourseSeller.Core.Security;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class AdminService : IAdminService
{
    private readonly MssqlContext _context;
    private readonly IAccountService _accountService;

    public AdminService(MssqlContext context, IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
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

    public async Task<List<Role>> GetAllRoles()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<string> CreateUser(CreateUserViewModel viewModel)
    {
        var user = new User
        {
            UserId = CodeGenerators.Generate32ByteUniqueCode(),
            Password = PasswordHelper.HashPassword(viewModel.Password),
            ActiveCode = CodeGenerators.Generate64ByteUniqueCode(),
            ActiveCodeGenerateDateTime = DateTime.Now,
            UserName = viewModel.UserName.ToLower(),
            Email = FixText.FixEmail(viewModel.Email),
            IsActive = viewModel.IsActive,
            RegisterDateTime = DateTime.Now,
        };

        // We had new image to upload
        if (viewModel.Avatar != null)
        {
            string imagePath = null;

            // Save new image
            var avatarName =
                $"{CodeGenerators.Generate32ByteUniqueCode()}{Path.GetExtension(viewModel.Avatar.FileName)}";
            imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars",
                avatarName);
            await using var stream = new FileStream(imagePath, FileMode.Create);
            await viewModel.Avatar.CopyToAsync(stream);
            user.UserAvatar = avatarName;

        }



        var userRoles = new List<UserRole>();
        foreach (var roleId in viewModel.SelectedRoles)
        {
            userRoles.Add(new UserRole()
            {
                RoleId = roleId,
                UserId = user.UserId
            });
        }

        await _context.Users.AddAsync(user);
        await _context.UserRoles.AddRangeAsync(userRoles);

        await _context.SaveChangesAsync();


        return user.UserId;
    }
}

