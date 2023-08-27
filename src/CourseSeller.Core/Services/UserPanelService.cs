using CourseSeller.Core.DTOs.UserPanel;
using CourseSeller.Core.DTOs.UserPanel.Wallet;
using CourseSeller.Core.Generators;
using CourseSeller.Core.Security;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Wallets;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class UserPanelService : IUserPanelService
{
    public const byte DEPOSIT_TYPEID = 1;
    public const byte WITHDRAWAL_TYPEID = 2;

    private readonly MssqlContext _context;
    private readonly IAccountService _accountService;

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
                RegisterDateTime = u.RegisterDateTime,
                Email = u.Email,
                WalletBalance = u.WalletBalance,
                SideBarViewModel = new()
                {
                    ImageName = u.UserAvatar,
                    RegisterDateTime = u.RegisterDateTime,
                    UserName = u.UserName,
                }
            });

        return await query.SingleOrDefaultAsync();
    }

    public async Task<UserInfoViewModel> GetUserInfoById(string userId)
    {
        var query = _context.Users.Where(u => u.UserId == userId)
            .Select(u => new UserInfoViewModel()
            {
                UserName = u.UserName,
                RegisterDateTime = u.RegisterDateTime,
                Email = u.Email,
                WalletBalance = u.WalletBalance
            });

        return await query.SingleOrDefaultAsync();
    }

    public async Task<SideBarViewModel> GetSideBarData(string userName)
    {
        var query = _context.Users.Where(u => u.UserName == userName)
            .Select(u => new SideBarViewModel()
            {
                UserName = u.UserName,
                ImageName = u.UserAvatar,
                RegisterDateTime = u.RegisterDateTime,
            });

        return await query.SingleOrDefaultAsync();
    }

    public async Task<EditProfileViewModel> GetDataForEditUserProfile(string userName)
    {
        return await _context.Users.Where(u => u.UserName == userName)
            .Select(u => new EditProfileViewModel()
            {
                AvatarName = u.UserAvatar,
                Email = u.Email,
                UserName = u.UserName,
                SideBarViewModel = new()
                {
                    ImageName = u.UserAvatar,
                    RegisterDateTime = u.RegisterDateTime,
                    UserName = u.UserName,
                }
            })
            .SingleAsync(); // return error when have more than one!
    }

    // Get username separately for when username changed
    public async Task<bool> EditProfile(string userName, EditProfileViewModel viewModel)
    {
        // We had new image to upload
        if (viewModel.UserAvatar != null)
        {
            string imagePath = null;
            // Delete old image
            if (viewModel.AvatarName != "Default.png")
            {
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars",
                    viewModel.AvatarName);
                // We can do soft delete and hold use old images in a folder for security purpose
                // BUG: We have roleback db on error but we havent it on delete file!
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }
            // Save new image
            viewModel.AvatarName =
                $"{CodeGenerators.Generate32ByteUniqueCode()}{Path.GetExtension(viewModel.UserAvatar.FileName)}";
            imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatars",
                viewModel.AvatarName);
            await using (var stream = new FileStream(imagePath, FileMode.Create))
                await viewModel.UserAvatar.CopyToAsync(stream);

        }
        // Edit user info
        var user = await _accountService.GetUserByUserName(userName);
        user.UserName = viewModel.UserName;
        // Email changed so send verification email
        if (user.Email != viewModel.Email)
        {
            user.IsActive = false;
            await _accountService.RevokeActiveCodeAndNewSendEmail(user);
        }
        user.Email = viewModel.Email;
        user.UserAvatar = viewModel.AvatarName;

        return await _accountService.UpdateUser(user);
    }

    public bool ImageHasValidExtension(string imageFileName, List<string> expectedExtensions = null)
    {
        // Initialize it with default standard values
        if (expectedExtensions == null)
            expectedExtensions = new List<string> { ".png", ".jpg" };

        var imageExtension = Path.GetExtension(imageFileName);
        if (expectedExtensions.Contains(imageExtension))
            return true;

        return false;
    }

    public async Task<bool> ChangePassword(string userName, string oldPassword, string newPassword)
    {
        var user = await _accountService.GetUserByUserName(userName);

        if (PasswordHelper.VerifyPassword(oldPassword, user.Password))
        {
            user.Password = PasswordHelper.HashPassword(newPassword);

            await _accountService.UpdateUser(user);

            return true;
        }

        return false;
    }


    #region Wallet

    public async Task<List<WalletViewModel>> GetUserWallet(string userName)
    {
        var userId = await _accountService.GetUserIdByUserName(userName);

        return await _context.Wallets
            .Where(w => w.UserId == userId && w.IsPaid == true)
            .Select(w => new WalletViewModel()
            {
                Amount = w.Amount,
                DateTime = w.CreatedDateTime,
                Description = w.Description,
                Type = w.TypeId
            })
            .OrderByDescending(w => w.DateTime)
            .ThenBy(w => w.Description)
            .ThenByDescending(w => w.Amount)
            .ToListAsync();
    }

    public async Task<int> AddWallet(string userName, int amount, string description = "شارژ حساب", bool isPaid = false)
    {
        var wallet = new Wallet()
        {
            Amount = +amount,
            CreatedDateTime = DateTime.Now,
            Description = description,
            IsPaid = isPaid,
            TypeId = DEPOSIT_TYPEID,
            UserId = await _accountService.GetUserIdByUserName(userName),
        };

        await _context.Wallets.AddAsync(wallet);
        await _context.SaveChangesAsync();

        return wallet.WalletId;
    }

    public async Task<Wallet> GetWalletById(int walletId)
    {
        return await _context.Wallets.FindAsync(walletId);
    }

    public async Task UpdateWallet(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task ChargeUserWallet(string userName, int amount)
    {
        var user = await _accountService.GetUserByUserName(userName);
        user.WalletBalance += amount;

        await _context.SaveChangesAsync();
    }

    public async Task SetWalletIsPaidAndChargeTransaction(Wallet wallet, string userName, int amount)
    {
        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Perform your database operations within the transaction
                // For example, you can add, update, or delete entities

                wallet.IsPaid = true;
                _context.Wallets.Update(wallet);
                var user = await _accountService.GetUserByUserName(userName);
                user.WalletBalance += amount;

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction if everything succeeds
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Handle exceptions or rollback the transaction if needed
                await transaction.RollbackAsync();
                throw; // Optional: rethrow the exception
            }
        }
    }

    #endregion
}
