using CourseSeller.Core.DTOs.UserPanel;
using CourseSeller.Core.DTOs.UserPanel.Wallet;
using CourseSeller.DataLayer.Entities.Wallets;

namespace CourseSeller.Core.Services.Interfaces;

public interface IUserPanelService
{
    Task<UserInfoViewModel> GetUserInfo(string userName);
    Task<SideBarViewModel> GetSideBarData(string userName);
    Task<EditProfileViewModel> GetDataForEditUserProfile(string userName);
    Task<bool> EditProfile(string userName, EditProfileViewModel viewModel);
    bool ImageHasValidExtension(string imageFileName, List<string> expectedExtensions = null);
    Task<bool> ChangePassword(string userName, string oldPassword, string newPassword);


    #region Wallet

    Task<List<WalletViewModel>> GetUserWallet(string userName);
    Task ChargeWallet(string userName, int amount, string description = "شارژ حساب", bool isPaid = false);
    Task AddWallet(Wallet wallet);

    #endregion
}