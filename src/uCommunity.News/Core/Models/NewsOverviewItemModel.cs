﻿using System;
using uCommunity.Core.User;

namespace uCommunity.News
{
    public class NewsOverviewItemModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Teaser { get; set; }

        public DateTime PublishDate { get; set; }

        public IntranetUserBase Creator { get; set; }

        public string MediaIds { get; set; }
    }
}