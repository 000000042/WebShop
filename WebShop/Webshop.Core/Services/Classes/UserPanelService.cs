using Webshop.Core.DTOs;
using Webshop.Core.Generators;
using Webshop.Core.Services.Interfaces;
using Webshop.DataLayer.Context;
using System.IO;
using System.Linq;
using Webshop.Core.Services.Classes;
using Webshop.Core.Security;

namespace Webshop.Core.Services.Classes
{
    public class UserPanelService : IUserPanelService
    {
        private WebshopContext _context;
        private IWalletService _walletService;

        public UserPanelService(WebshopContext context, IWalletService walletService)
        {
            _context = context;
            _walletService = walletService;
        }

        public void ChangePassword(int userId, string newPassword)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == userId);
            user.Password = PasswordHelper.EncodePasswordMd5(newPassword);

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public bool CompareOldPassword(int userId, string oldPassword)
        {
            var userSalt = _context.Users.SingleOrDefault(u => u.UserId == userId).Salt;

            string hashPassword = PasswordHelper.EncodePassword(oldPassword, userSalt);

            return _context.Users.Any(u => u.UserId == userId && u.Password == hashPassword);
        }

        public void EditProfile(int userId, EditProfileViewModel profile)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == userId);

            if (profile.UserAvatar != null)
            {
                string imagePath = "";
                if (user.UserAvatar != "DefaultAvatar.jpg")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UsersAvatar", user.UserAvatar);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
                profile.AvatarName = GuidGenerator.ActiveCodeGenerator() + Path.GetExtension(profile.UserAvatar.FileName);

                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UsersAvatar", profile.AvatarName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    profile.UserAvatar.CopyTo(stream);
                }
            }
            user.UserName = profile.UserName;
            user.Email = profile.Email;
            user.UserAvatar = profile.AvatarName;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public UserPanelSideBarViewModel GetSideBarData(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId)
                .Select(u => new UserPanelSideBarViewModel()
                {
                    Username = u.UserName,
                    RegisterDate = u.RegisterDate,
                    ImageName = u.UserAvatar
                }).Single();
        }

        public EditProfileViewModel GetUserForEdit(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId)
                .Select(u => new EditProfileViewModel()
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    AvatarName = u.UserAvatar
                }).Single();
        }

        public UserInformationViewModel GetUserInformation(int userId)
        {
            var user = _context.Users
                .SingleOrDefault(u => u.UserId == userId);

            UserInformationViewModel information = new UserInformationViewModel()
            {
                Username = user.UserName,
                Email = user.Email,
                RegisterDate = user.RegisterDate,
                Wallet = _walletService.UserWalletAmount(userId)
            };

            return information;
        }
    }
}
