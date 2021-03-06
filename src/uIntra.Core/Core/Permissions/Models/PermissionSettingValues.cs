﻿namespace Uintra.Core.Permissions.Models
{
    public class PermissionSettingValues
    {
        public bool IsAllowed { get; }
        public bool IsEnabled { get; }

        public PermissionSettingValues(bool isAllowed, bool isEnabled)
        {
            IsAllowed = isAllowed;
            IsEnabled = isEnabled;
        }

        public static PermissionSettingValues Of(bool isAllowed, bool isEnabled) =>
            new PermissionSettingValues(isAllowed, isEnabled);
    }
}