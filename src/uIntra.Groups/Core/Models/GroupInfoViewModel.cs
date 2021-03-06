﻿using System;
using Uintra.Core.User;

namespace Uintra.Groups
{
    public class GroupInfoViewModel
    {
        public Guid Id { get; set; }        
        public string GroupImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public MemberViewModel Creator { get; set; }
        public bool IsMember { get; set; }
        public int MembersCount { get; set; }
        public bool CanUnsubscribe { get; set; }
        public string CreatorProfileUrl { get; set; }
    }
}