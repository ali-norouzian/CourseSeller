using CourseSeller.Core.DTOs.Admin;
using CourseSeller.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace CourseSeller.Core.Services.Interfaces;

public interface IAdminService
{
    Task<UsersViewModel> GetAllUsers(int pageId = 1, string filterEmail = "", string filterUserName = "");
    Task<UsersViewModel> GetAllDeletedUsers(int pageId = 1, string filterEmail = "", string filterUserName = "");
    Task<List<Role>> GetAllRoles();
    Task<string> CreateUser(CreateUserViewModel viewModel);
    Task<string> UploadNewAvatar(string oldAvatarName, IFormFile avatar = null);
    Task<EditUserViewModel> GetUserInfoForUpdate(string userId);
    Task UpdateUser(EditUserViewModel viewModel);
    Task SoftDeleteUser(string userId);
}