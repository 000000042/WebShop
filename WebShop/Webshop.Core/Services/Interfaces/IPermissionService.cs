using Webshop.DataLayer.Entities.Permission;
using Webshop.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Roles

        List<Role> GetRoles();
        void AddRolesForUser(List<int> SelectedRoles, int userId);
        void EditUserRoles(int userId, List<int> SelectedRoles);
        int AddRoleByAdmin(Role role);
        void DeleteRoleByAdmin(Role role);
        Role GetRoleForAdmin(int roleId);
        void EditRoleByAdmin(Role role);

        #endregion

        #region Permissions

        List<Permission> GetAllPermissions();
        void AddPermissionsToRole(int roleId, List<int> SelectedPermissions);
        void EditPermissionsToRole(int roleId, List<int> SelectedPermissions);

        #endregion

        #region PermissionChecker

        bool CheckUserPermission(int permissionId, string userName);

        #endregion
    }
}
