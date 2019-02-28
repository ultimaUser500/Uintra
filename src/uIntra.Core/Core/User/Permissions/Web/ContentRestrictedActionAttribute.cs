﻿using System;
using System.Web;
using System.Web.Mvc;
using Uintra.Core.Extensions;
using Uintra.Core.Permissions;
using Uintra.Core.Permissions.Interfaces;
using Uintra.Core.Permissions.Models;

namespace Uintra.Core.User.Permissions.Web
{
    public class ContentRestrictedActionAttribute : ActionFilterAttribute
    {
        private readonly PermissionSettingIdentity _permissionSettingIdentity;

        public ContentRestrictedActionAttribute(Enum resourceType, Enum actionType)
        {
            _permissionSettingIdentity = PermissionSettingIdentity.Of(actionType, resourceType);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var permissionsService = HttpContext.Current.GetService<IPermissionsService>();
            var isUserHasAccess = permissionsService.Check(_permissionSettingIdentity);

            if (!isUserHasAccess)
            {
                Deny(filterContext);
            }
        }

        private void Deny(ActionExecutingContext filterContext)
        {
            filterContext.Result = new EmptyResult();
        }
    }
}
