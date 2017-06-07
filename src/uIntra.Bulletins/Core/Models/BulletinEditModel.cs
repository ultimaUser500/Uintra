using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using uIntra.Core.Attributes;
using uIntra.Core.Media;
using uIntra.Core.ModelBinders;

namespace uIntra.Bulletins
{
    public class BulletinEditModel : IContentWithMediaCreateEditModel
    {
        [Required]
        public virtual Guid Id { get; set; }

        [RequiredIfEmpty(OtherProperty = nameof(NewMedia)), AllowHtml, StringLength(2000)]
        public string Description { get; set; }

        [PropertyBinder(typeof(DateTimeBinder))]
        public DateTime PublishDate { get; set; }

        public string Media { get; set; }

        [Required]
        public int? MediaRootId { get; set; }

        [RequiredIfEmpty(OtherProperty = nameof(Description))]
        public string NewMedia { get; set; }

        public Guid CreatorId { get; set; }
    }
}