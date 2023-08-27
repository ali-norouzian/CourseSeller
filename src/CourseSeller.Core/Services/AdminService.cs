using CourseSeller.Core.Convertors;
using CourseSeller.Core.DTOs.Admin;
using CourseSeller.Core.Generators;
using CourseSeller.Core.Security;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class AdminService : IAdminService
{
    private readonly MssqlContext _context;
    private readonly IAccountService _accountService;
    private readonly IPasswordHelper _passwordHelper;

    public AdminService(MssqlContext context, IAccountService accountService, IPasswordHelper passwordHelper)
    {
        _context = context;
        _accountService = accountService;
        _passwordHelper = passwordHelper;
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

    public async Task<UsersViewModel> GetAllDeletedUsers(int pageId = 1, string filterEmail = "", string filterUserName = "")
    {
        // Ignore qfs
        IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u=>u.IsDelete==true);

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
            Password = await _passwordHelper.HashPassword(viewModel.Password),
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
            await using (var stream = new FileStream(imagePath, FileMode.Create))
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

    public async Task<string> UploadNewAvatar(string oldAvatarName, IFormFile avatar = null)
    {
        var avatarName = avatar.Name;
        // We had new image to upload
        if (avatar != null)
        {
            string imagePath = null;
            // Delete old image
            if (oldAvatarName != "Default.png")
            {
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars",
                    oldAvatarName);
                // We can do soft delete and hold use old images in a folder for security purpose
                // BUG: We have roleback db on error but we havent it on delete file!
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }
            // Save new image
            avatarName =
                $"{CodeGenerators.Generate32ByteUniqueCode()}{Path.GetExtension(avatar.FileName)}";
            imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars",
                avatarName);
            await using (var stream = new FileStream(imagePath, FileMode.Create))
                await avatar.CopyToAsync(stream);
        }

        return avatarName;
    }

    public async Task<EditUserViewModel> GetUserInfoForUpdate(string userId)
    {
        return await _context.Users.Where(u => u.UserId == userId)
            .Select(u => new EditUserViewModel()
            {
                UserId = userId,
                AvatarName = u.UserAvatar,
                Email = u.Email,
                UserName = u.UserName,
                IsActive = u.IsActive,
                SelectedRoles = u.UserRoles
                    .Select(r => r.RoleId)
                    .ToList(),
            }).SingleAsync();
    }

    // It can be better...
    public async Task UpdateUser(EditUserViewModel viewModel)
    {
        var user = await _context.Users.FindAsync(viewModel.UserId);
        user.Email = viewModel.Email;
        user.UserName = viewModel.UserName;
        user.IsActive = viewModel.IsActive;
        if (!string.IsNullOrEmpty(viewModel.Password))
            user.Password = await _passwordHelper.HashPassword(viewModel.Password);

        if (viewModel.Avatar != null)
        {
            //Delete old Image
            if (viewModel.AvatarName != "Defult.jpg")
            {
                string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars", viewModel.AvatarName);
                if (File.Exists(deletePath))
                    File.Delete(deletePath);
            }

            //Save New Image
            user.UserAvatar = CodeGenerators.Generate32ByteUniqueCode() + Path.GetExtension(viewModel.Avatar.FileName);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars", user.UserAvatar);
            await using (var stream = new FileStream(imagePath, FileMode.Create))
                await viewModel.Avatar.CopyToAsync(stream);
            
        }

        //user.UserAvatar = viewModel.AvatarName;

        // Delete All User Roles
        _context.UserRoles.Where(r => r.UserId == user.UserId).ToList().ForEach(r => _context.UserRoles.Remove(r));

        // Add Roles to user
        var userRoles = new List<UserRole>();
        // Select 0 role
        if (viewModel.SelectedRoles != null)
        {
            foreach (var roleId in viewModel.SelectedRoles)
            {
                userRoles.Add(new UserRole()
                {
                    RoleId = roleId,
                    UserId = user.UserId
                });
            }
        }

        await _context.UserRoles.AddRangeAsync(userRoles);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

    }

    public async Task SoftDeleteUser(string userId)
    {
        var user =await _accountService.GetUserByUserId(userId);
        user.IsDelete=true;

        await _accountService.UpdateUser(user);
    }
}

