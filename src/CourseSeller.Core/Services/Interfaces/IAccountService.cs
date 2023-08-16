using CourseSeller.DataLayer.Entities.Users;

namespace CourseSeller.Core.Services.Interfaces;

public interface IAccountService
{
    Task<bool> IsExistUserName(string userName);
    Task<bool> IsExistEmail(string email);
    Task<User> AddUser(User user);
    Task<User> GetUserByEmail(string email);
    Task<User> GetUserByUserName(string userName);
    Task<byte> ActiveAccount(string userId, string activeCode);
    Task<bool> UpdateUser(User user);
    Task<bool> RevokeActiveCodeAndNewSendEmail(User user, string emailBody = "Emails/_ActivateEmail", string emailSubject = "فعالسازی");

}