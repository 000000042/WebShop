using Webshop.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.Services.Interfaces
{
    public interface IUserPanelService
    {
        #region Get

        UserInformationViewModel GetUserInformation(int userId);
        UserPanelSideBarViewModel GetSideBarData(int userId);
        EditProfileViewModel GetUserForEdit(int userId);
        bool CompareOldPassword(int userId, string oldPassword);

        #endregion

        #region Add



        #endregion

        #region Edit

        void EditProfile(int userId, EditProfileViewModel profile);
        void ChangePassword(int userId, string newPassword);

        #endregion

        #region Remove



        #endregion
    }
}
