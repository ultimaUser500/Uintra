﻿using System;
using ServiceStack.DataAnnotations;
using uCommunity.Core.Persistence.Sql;

namespace uCommunity.Navigation.Core
{
    [CompositeIndex("UserId", "ContentId", Unique = true, Name = "UQ_MyLink_UserId_ContentId")]
    public class MyLink : SqlEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public int ContentId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}