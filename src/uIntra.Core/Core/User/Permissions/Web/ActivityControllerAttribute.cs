﻿using System.Web.Mvc;

namespace uIntra.Core.User.Permissions.Web
{
    public class ActivityControllerAttribute : ActionFilterAttribute
    {
        public int ActivityType { get; }

        public ActivityControllerAttribute(int type)
        {
            ActivityType = type;
        }
    }
}