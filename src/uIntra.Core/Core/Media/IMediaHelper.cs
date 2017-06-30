using System.Collections.Generic;
using uIntra.Core.Controls.FileUpload;
using Umbraco.Core.Models;

namespace uIntra.Core.Media
{
    public interface IMediaHelper
    {
        IEnumerable<int> CreateMedia(IContentWithMediaCreateEditModel model);
        void DeleteMedia(int mediaId);
        void DeleteMedia(IEnumerable<int> mediaIds);
        IMedia CreateMedia(TempFile file, int rootMediaId);
        MediaSettings GetMediaFolderSettings(MediaFolderTypeEnum mediaFolderType);
        bool IsMediaDeleted(IPublishedContent media);
    }
}