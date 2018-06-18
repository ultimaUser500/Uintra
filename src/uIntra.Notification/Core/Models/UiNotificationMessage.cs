﻿using System;

namespace Uintra.Notification
{
    public class UiNotificationMessage : INotificationMessage
    {
        public Enum NotificationType { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid? NotifierId { get; set; }
        public string Url { get; set; }
        public string Message { get; set; }
        public bool IsPinned { get; set; }
        public bool IsPinActual { get; set; }
    }
}
