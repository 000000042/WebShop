using Webshop.Core.Conventors;
using Webshop.Core.DTOs;
using Webshop.Core.Generators;
using Webshop.Core.Security;
using Webshop.Core.Services.Interfaces;
using Webshop.DataLayer.Context;
using Webshop.DataLayer.Entities.User;
using SendEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webshop.Core.Services.Classes
{
    public class AccountService : IAccountService
    {
        private WebshopContext _context;
        private IViewRenderService _viewRender;

        public AccountService(WebshopContext context, IViewRenderService viewRender)
        {
            _context = context;
            _viewRender = viewRender;
        }

        public int AddUser(User newUser)
        {
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return newUser.UserId;
        }

        public User GetUserByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }

        public bool IsActiveCodeValid(string activeCode)
        {
            if (_context.Users.Any(u => u.ActiveCode == activeCode))
            {
                var user = _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
                if (!user.IsActive)
                {
                    user.IsActive = true;
                    user.ActiveCode = GuidGenerator.ActiveCodeGenerator();

                    _context.Users.Update(user);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsExistEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public bool IsExistUsername(string username)
        {
            return _context.Users.Any(u => u.UserName == username);
        }

        public User LoginUser(LoginViewModel login)
        {
            var userSalt = _context.Users
                .Where(u => u.Email == login.Email)
                .Select(u => u.Salt).SingleOrDefault();

            if (userSalt == null)
            {
                return null;
            }

            var password = PasswordHelper.EncodePassword(login.Password, userSalt);

            var email = FixedText.FixedEmail(login.Email);

            User user = _context.Users
                .SingleOrDefault(u => u.Email == email && u.Password == password);

            return user;
        }

        public void EmailSender(string emailView, User user, string subject)
        {
                string body = _viewRender.RenderToStringAsync(emailView, user);
                SendEmail.SendEmail.Send(user.Email, subject, body);
        }

        public bool UpdateUser(User user)
        {
            if (user != null)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetUserIdByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(u => !u.IsDelete && u.UserName == userName).UserId;
        }
    }
}
