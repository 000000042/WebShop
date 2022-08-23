using Webshop.Core.DTOs;
using Webshop.DataLayer.Entities.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.Services.Interfaces
{
    public interface IAdminService
    {
        #region Get

        ShowUsersViewModel GetAllUsers(int pageId = 1, string filterEmail = "", string filetUsername = "");
        EditUserViewModel GetUserForEditByAdmin(int userId);
        User GetUserById(int userId);
        DeleteUserViewModel GetUserForDeleteByAdmin(int userId);
        ShowUsersViewModel GetAllDeletedUsers(int pageId = 1, string filterEmail = "", string filetUsername = "");
        DeleteUserViewModel GetUserForRestoreByAdmin(int userId);

        #endregion

        #region Add

        int AddUserByAdmin(CreateUserViewModel user);
        void SaveImage(User user, IFormFile userNewAvatar);

        #endregion

        #region Edit

        void EditUserByAdmin(EditUserViewModel editUser);
        void EditUserRoles(int userId, List<int> SelectedRoles);
        void RestoreUserByAdmin(int userId);

        #endregion

        #region Remove

        void DeleteUser(int userId);

        #endregion
    }
}
