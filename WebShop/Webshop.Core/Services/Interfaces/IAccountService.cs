using Webshop.Core.DTOs;
using Webshop.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Webshop.Core.Services.Interfaces
{
    public interface IAccountService
    {
        #region Get

        bool IsExistUsername(string username);
        bool IsExistEmail(string email);
        User LoginUser(LoginViewModel login);
        User GetUserByEmail(string email);
        User GetUserByActiveCode(string activeCode);
        bool IsActiveCodeValid(string activeCode);
        int GetUserIdByUserName(string userName);

        #endregion

        #region Add

        int AddUser(User newUser);
        void EmailSender(string emailView, User user, string subject);

        #endregion

        #region Edit

        bool UpdateUser(User user);

        #endregion

        #region Remove



        #endregion
    }
}
