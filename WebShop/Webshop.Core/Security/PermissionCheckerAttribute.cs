using Webshop.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;

namespace Webshop.Core.Security
{
    public class PermissionCheckerAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private int _permissionId = 0;
        private IPermissionService _permissionService;

        public PermissionCheckerAttribute(int permissionId)
        {
            _permissionId = permissionId;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                _permissionService =
                    (IPermissionService)context.HttpContext.RequestServices.GetService(typeof(IPermissionService));

                string userName = context.HttpContext.User.Identity.Name;

                if (!_permissionService.CheckUserPermission(_permissionId, userName))
                {
                    context.Result = new RedirectResult("/Home?" + context.HttpContext.Request.Path);
                }
            }
            else
            {
                context.Result = new RedirectResult("/Login" + "?ReturnPath=" + context.HttpContext.Request.Path);
            }
        }
    }
}
