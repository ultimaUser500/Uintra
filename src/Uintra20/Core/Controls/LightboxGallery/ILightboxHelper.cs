﻿using System.Collections.Generic;
using Uintra20.Features.Media.Strategies.ImageResize;

namespace Uintra20.Core.Controls.LightboxGallery
{
    public interface ILightboxHelper
    {
        void FillGalleryPreview(IHaveLightboxPreview model, IEnumerable<int> mediaIds);
        LightboxPreviewModel GetGalleryPreviewModel(IEnumerable<int> mediaIds, IImageResizeStrategy strategy);
    }
}
