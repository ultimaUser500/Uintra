﻿using System;

namespace uIntra.Subscribe
{
    public class SubscribeNotificationDisableUpdateModel
    {
        public Guid Id { get; set; }

        public bool NewValue { get; set; }

        public Guid ActivityId { get; set; }
    }
}