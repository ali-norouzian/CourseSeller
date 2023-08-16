using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.Core.Services.Interfaces;

public interface IAccountService
{
    Task<bool> IsExistUserName(string userName);
    Task<bool> IsExistEmail(string email);
    Task<User> AddUser(User user);
    Task<User> GetUserByEmail(string email);
    Task<User> GetUserByUserName(string userName);
    Task<bool> UpdateUser(User user);
    Task<byte> ActiveAccount(string userId, string activeCode);
    Task<bool> RevokeActiveCodeAndNewSendEmail(User user, string emailBody = "Emails/_ActivateEmail", string emailSubject = "فعالسازی");
    Task<bool> RevokeActiveCode(User user);
    Task<User> GetUserByActiveCode(string userId, string activeCode);
    Task<bool> ResetPassword(User user, string newPassword);
    Task<string> GetUserIdByUserName(string userName);
}