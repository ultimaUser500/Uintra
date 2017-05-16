﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using uCommunity.CentralFeed;
using uCommunity.Comments;
using uCommunity.Events;
using uCommunity.Likes;
using uCommunity.Subscribe;
using uCommunity.Tagging;

namespace Compent.uCommunity.Core.Events
{
    public class Event : EventBase, ICentralFeedItem, ICommentable, ILikeable, ISubscribable, IHaveTags
    {
        [JsonIgnore]
        public DateTime SortDate => PublishDate;
        [JsonIgnore]
        public IEnumerable<LikeModel> Likes { get; set; }
        [JsonIgnore]
        public IEnumerable<Comment> Comments { get; set; }
        [JsonIgnore]
        public IEnumerable<global::uCommunity.Subscribe.Subscribe> Subscribers { get; set; }
        [JsonIgnore]
        public IEnumerable<string> Tags { get; set; }
    }
}