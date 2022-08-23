using Webshop.Core.DTOs;
using Webshop.Core.Generators;
using Webshop.Core.Security;
using Webshop.Core.Services.Interfaces;
using Webshop.DataLayer.Context;
using Webshop.DataLayer.Entities.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Webshop.Core.Services.Classes
{
    public class AdminService : IAdminService
    {
        private WebshopContext _context;
        private IAccountService _accountService;
        private IWalletService _walletService;

        public AdminService(WebshopContext context, IAccountService accountService, IWalletService walletService)
        {
            _context = context;
            _accountService = accountService;
            _walletService = walletService;
        }

        public int AddUserByAdmin(CreateUserViewModel user)
        {
            User addUser = new User();
            var salt = PasswordHelper.SaltGenerator();

            addUser.UserName = user.UserName;
            addUser.Password = PasswordHelper.EncodePassword(user.Password, salt);
            addUser.Email = user.Email;
            addUser.RegisterDate = DateTime.Now;
            addUser.Salt = salt;
            addUser.IsActive = true;
            addUser.ActiveCode = GuidGenerator.ActiveCodeGenerator();

            if (user.UserAvatar != null)
            {
                string imagePath = "";
                addUser.UserAvatar = GuidGenerator.ActiveCodeGenerator() + Path.GetExtension(user.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UsersAvatar", addUser.UserAvatar);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    user.UserAvatar.CopyTo(stream);
                }
            }
            else
                addUser.UserAvatar = "DefaultAvatar.jpg";

            return _accountService.AddUser(addUser);
        }

        public void DeleteUser(int userId)
        {
            User user = GetUserById(userId);

            user.IsDelete = true;
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void EditUserByAdmin(EditUserViewModel editUser)
        {
            User user = GetUserById(editUser.UserId);
            user.Email = editUser.Email;
            user.IsActive = editUser.IsActive;

            if (editUser.Password != null)
                user.Password = PasswordHelper.EncodePassword(editUser.Password, user.Salt);

            if (editUser.UserAvatar != null)
            {
                if (user.UserAvatar != "DefaultAvatar.jpg")
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UsersAvatar", editUser.AvatarName);

                    if (File.Exists(deletePath))
                    {
                        File.Delete(deletePath);
                    }
                }
                SaveImage(user, editUser.UserAvatar);
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void EditUserRoles(int userId, List<int> SelectedRoles)
        {
            _context.UserRoles.Where(u => u.UserId == userId).ToList().ForEach(r => _context.UserRoles.Remove(r));
        }

        public ShowUsersViewModel GetAllDeletedUsers(int pageId = 1, string filterEmail = "", string filetUsername = "")
        {
            IQueryable<User> users = _context.Users.IgnoreQueryFilters().Where(u => u.IsDelete);

            if (!string.IsNullOrEmpty(filetUsername))
            {
                users = users.Where(u => u.UserName.Contains(filetUsername));
            }

            if (!string.IsNullOrEmpty(filterEmail))
            {
                users = users.Where(u => u.UserName.Contains(filterEmail));
            }


            int take = 5;
            int skip = (pageId - 1) * take;

            ShowUsersViewModel pageUsers = new ShowUsersViewModel();
            pageUsers.CurrentPage = pageId;
            pageUsers.PageCount = _context.Users.Count() / take;
            pageUsers.Users = users.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return pageUsers;
        }

        public ShowUsersViewModel GetAllUsers(int pageId = 1, string filterEmail = "", string filetUsername = "")
        {
            IQueryable<User> users = _context.Users;

            if (!string.IsNullOrEmpty(filetUsername))
            {
                users = users.Where(u => u.UserName.Contains(filetUsername));
            }

            if (!string.IsNullOrEmpty(filterEmail))
            {
                users = users.Where(u => u.UserName.Contains(filterEmail));
            }


            int take = 5;
            int skip = (pageId - 1) * take;

            ShowUsersViewModel pageUsers = new ShowUsersViewModel();
            pageUsers.CurrentPage = pageId;
            pageUsers.PageCount = _context.Users.Count() / take;
            pageUsers.Users = users.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return pageUsers;
        }

        public User GetUserById(int userId)
        {
            return _context.Users.Single(u => u.UserId == userId);
        }

        public DeleteUserViewModel GetUserForDeleteByAdmin(int userId)
        {
            DeleteUserViewModel user = _context.Users.Include(w => w.Wallets).Where(u => u.UserId == userId)
                .Select(u => new DeleteUserViewModel()
                {
                    UserId = u.UserId,
                    Username = u.UserName,
                    Email = u.Email,
                    RegisterDate = u.RegisterDate,
                    IsActive = u.IsActive,
                    AvatarName = u.UserAvatar,
                    WalletAmount = _walletService.UserWalletAmount(u.UserId)
                }).Single();

            return user;
        }

        public EditUserViewModel GetUserForEditByAdmin(int userId)
        {
            return _context.Users
                .Include(u => u.UserRoles)
                .Where(u => u.UserId == userId)
                .Select(u => new EditUserViewModel()
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    AvatarName = u.UserAvatar,
                    IsActive = u.IsActive,
                    UserRoles = u.UserRoles.Select(r => r.RoleId).ToList()
                }).Single();
        }

        public DeleteUserViewModel GetUserForRestoreByAdmin(int userId)
        {
            DeleteUserViewModel user = _context.Users.IgnoreQueryFilters().Include(w => w.Wallets)
                .Where(u => u.UserId == userId)
                .Select(u => new DeleteUserViewModel()
                {
                    UserId = u.UserId,
                    Username = u.UserName,
                    Email = u.Email,
                    AvatarName = u.UserAvatar,
                    IsActive = u.IsActive,
                    RegisterDate = u.RegisterDate,
                    WalletAmount = _walletService.UserWalletAmount(u.UserId)
                }).Single();

            return user;
        }

        public void RestoreUserByAdmin(int userId)
        {
            User user = _context.Users.IgnoreQueryFilters().SingleOrDefault(u => u.UserId == userId);
            user.IsDelete = false;

            _context.Users.Update(user);
            _context.SaveChanges();
         }

        public void SaveImage(User user, IFormFile userNewAvatar)
        {
            user.UserAvatar = GuidGenerator.ActiveCodeGenerator() + Path.GetExtension(userNewAvatar.FileName);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UsersAvatar", user.UserAvatar);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                userNewAvatar.CopyTo(stream);
            }
        }
    }
}
