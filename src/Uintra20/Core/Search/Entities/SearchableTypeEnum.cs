﻿using Uintra20.Core.Activity;

namespace Uintra20.Core.Search.Entities
{
    public enum SearchableTypeEnum
    {
        News = IntranetActivityTypeEnum.News,
        Events = IntranetActivityTypeEnum.Events,
        Socials = IntranetActivityTypeEnum.Social,
        Content,
        Document
    }
}