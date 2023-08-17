using CourseSeller.Core.DTOs.UserPanel;

namespace CourseSeller.Core.Services.Interfaces;

public interface IUserPanelService
{
    Task<UserInfoViewModel> GetUserInfo(string userName);
    Task<SideBarViewModel> GetSideBarData(string userName);
    Task<EditProfileViewModel> GetDataForEditUserProfile(string userName);
    Task<bool> EditProfile(string userName, EditProfileViewModel viewModel);
    bool ImageHasValidExtension(string imageFileName, List<string> expectedExtensions = null);
    Task<bool> ChangePassword(string userName, string oldPassword, string newPassword);

}