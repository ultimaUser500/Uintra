﻿using UBaseline.Shared.Node;
using UBaseline.Shared.PageSettings;
using UBaseline.Shared.Property;
using Uintra20.Core.UbaselineModels.RestrictedNode;

namespace Uintra20.Features.News.Models
{
    public class UintraNewsCreatePageViewModel : UintraRestrictedNodeViewModel
    {
        public PropertyViewModel<INodeViewModel[]> Panels { get; set; }
        public PageSettingsCompositionViewModel PageSettings { get; set; }
        public NewsCreateDataViewModel Data { get; set; }
    }
}