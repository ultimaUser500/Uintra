﻿using System.ComponentModel.DataAnnotations;

namespace uIntra.Core.Attributes
{
    public class RequiredVirtualAttribute : RequiredAttribute
    {
        public bool IsRequired { get; set; } = true;

        public override bool IsValid(object value)
        {
            if (IsRequired)
                return base.IsValid(value);
            return true;
        }

        public override bool RequiresValidationContext => IsRequired;
    }
}