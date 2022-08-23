using Webshop.Core.Services.Interfaces;
using Webshop.DataLayer.Context;
using Webshop.DataLayer.Entities.Permission;
using Webshop.DataLayer.Entities.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webshop.Core.Services.Classes
{
    public class PermissionService : IPermissionService
    {
        private WebshopContext _context;

        public PermissionService(WebshopContext context)
        {
            _context = context;
        }

        public int AddRoleByAdmin(Role role)
        {
            role.IsDelete = false;
            _context.Roles.Add(role);
            _context.SaveChanges();

            return role.RoleId;
        }

        public void AddRolesForUser(List<int> SelectedRoles, int userId)
        {
            foreach (var role in SelectedRoles)
            {
                _context.UserRoles.Add(new UserRole()
                {
                    RoleId = role,
                    UserId = userId
                });
                _context.SaveChanges();
            }
        }

        public void EditUserRoles(int userId, List<int> SelectedRoles)
        {
            _context.UserRoles.Where(u => u.UserId == userId).ToList().ForEach(r => _context.UserRoles.Remove(r));

            AddRolesForUser(SelectedRoles, userId);
        }

        public Role GetRoleForAdmin(int roleId)
        {
            return _context.Roles.Include(p => p.RolePermissions).SingleOrDefault(r => r.RoleId == roleId);
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }

        public void EditRoleByAdmin(Role role)
        {
            _context.Roles.Update(role);

            _context.SaveChanges();
        }

        public void DeleteRoleByAdmin(Role role)
        {
            role.IsDelete = true;
            EditRoleByAdmin(role);
        }

        public List<Permission> GetAllPermissions()
        {
            return _context.Permissions.ToList();
        }

        public void AddPermissionsToRole(int roleId, List<int> SelectedPermissions)
        {
            foreach (var perm in SelectedPermissions)
            {
                _context.RolePermissions.Add(new RolePermission()
                {
                    RoleId = roleId,
                    PermissionId = perm
                });
            }
            _context.SaveChanges();
        }

        public void EditPermissionsToRole(int roleId, List<int> SelectedPermissions)
        {
            _context.RolePermissions.Where(r => r.RoleId == roleId)
                .ToList().ForEach(r => _context.Remove(r));

            AddPermissionsToRole(roleId, SelectedPermissions);
        }

        public bool CheckUserPermission(int permissionId, string userName)
        {
            int userId = _context.Users.SingleOrDefault(u => u.UserName == userName).UserId;

            List<int> UserRoles = _context.UserRoles.Where(u => u.UserId == userId)
                .Select(r => r.RoleId).ToList();

            if (!UserRoles.Any())
                return false;

            List<int> RolePermissions = _context.RolePermissions
                .Where(p => p.PermissionId == permissionId)
                .Select(r => r.RoleId).ToList();

            return RolePermissions.Any(p => RolePermissions.Contains(p));
        }
    }
}
