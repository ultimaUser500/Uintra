﻿using Uintra20.Core.Extensions;

namespace Uintra20.Core.Helpers
{
    public class HtmlHelper
    {
        public static string CreateLink(string title, string url, bool openInNewTab = false)
        {
            var link = $"<a href=\"{url.ToAbsoluteUrl()}\" target=\"{(openInNewTab ? "_blank " : "_self")}\">{title}</a>";
            return link;
        }
    }
}