using CourseSeller.Core.DTOs.UserPanel;

namespace CourseSeller.Core.Services.Interfaces;

public interface IUserPanelService
{
    Task<UserInfoViewModel> GetUserInfo(string userName);

}