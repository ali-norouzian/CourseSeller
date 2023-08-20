using CourseSeller.Core.DTOs.Admin;
using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.Core.Services.Interfaces;

public interface IAdminService
{
    Task<UsersViewModel> GetAllUsers(int pageId = 1, string filterEmail = "", string filterUserName = "");
}