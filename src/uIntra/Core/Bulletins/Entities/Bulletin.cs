﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using uIntra.Bulletins;
using uIntra.CentralFeed;
using uIntra.Comments;
using uIntra.Likes;
using uIntra.Subscribe;

namespace uIntra.Core.Bulletins
{
    public class Bulletin : BulletinBase, ICentralFeedItem, ICommentable, ILikeable, ISubscribable
    {
        [JsonIgnore]
        public DateTime SortDate => PublishDate;
        [JsonIgnore]
        public IEnumerable<LikeModel> Likes { get; set; }
        [JsonIgnore]
        public IEnumerable<Comment> Comments { get; set; }

        public IEnumerable<global::uIntra.Subscribe.Subscribe> Subscribers { get; set; }
        
    }
}