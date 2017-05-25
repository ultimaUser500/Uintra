﻿using System;
using System.Collections.Generic;
using uIntra.Core.Activity;

namespace uCommunity.Comments
{
    public interface ICommentable
    {
        Guid Id { get; }

        IntranetActivityTypeEnum Type { get; }

        IEnumerable<Comment> Comments { get; set; }
    }
}
