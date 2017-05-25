﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;

namespace uIntra.Core.ModelBinders
{
    public class DateTimeBinder : ICustomModelBinder
    {
        public object BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var value = bindingContext.ValueProvider.GetValue(propertyDescriptor.Name);
            bindingContext.ModelState.SetModelValue(propertyDescriptor.Name, value);

            if (value == null)
            {
                return null;
            }

            DateTime result;
            if (DateTime.TryParseExact(value.AttemptedValue, new[] { "yyyy-MM-ddTHH:mm:ss.fffZ", "yyyy-MM-ddTHH:mm:ss.fffzzz" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            return value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
        }
    }
}