﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace uIntra.Likes
{
    public class LikesViewModel
    {
        public Guid UserId { get; set; }

        public Guid ActivityId { get; set; }

        public Guid? CommentId { get; set; }

        public int Count { get; set; }

        public bool CanAddLike { get; set; }

        public IEnumerable<string> Users { get; set; }

        public bool IsReadOnly { get; set; }

        public LikesViewModel()
        {
            Users = Enumerable.Empty<string>();
        }
    }
}