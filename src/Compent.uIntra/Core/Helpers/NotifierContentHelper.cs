﻿using Compent.Uintra.Core.Events;
using System;
using Uintra.Comments;
using Uintra.Core.Activity;
using Uintra.Core.Extensions;
using Uintra.Core.Links;
using Uintra.Groups;
using Uintra.Notification;
using Uintra.Notification.Configuration;
using Uintra.Notification.Core.Entities;
using Umbraco.Core.Models;

namespace Compent.Uintra.Core.Helpers
{
    public class NotifierDataHelper : INotifierDataHelper
    {
        private readonly IActivityLinkService _linkService;
        private readonly ICommentLinkHelper _commentLinkHelper;
        private readonly IGroupService _groupService;

        const int MaxTitleLength = 100;

        public NotifierDataHelper(
            IActivityLinkService linkService, 
            ICommentLinkHelper commentLinkHelper,
            IGroupService groupService)
        {
            _linkService = linkService;
            _commentLinkHelper = commentLinkHelper;
            _groupService = groupService;
        }

        public CommentNotifierDataModel GetCommentNotifierDataModel(IIntranetActivity activity, CommentModel comment, Enum notificationType, Guid notifierId)
        {
            return new CommentNotifierDataModel
            {
                CommentId = comment.Id,
                NotificationType = notificationType,
                NotifierId = notifierId,
                Title = GetNotifierDataTitle(activity).TrimByWordEnd(MaxTitleLength),
                Url = _commentLinkHelper.GetDetailsUrlWithComment(activity.Id, comment.Id),
                IsPinned = activity.IsPinned,
                IsPinActual = activity.IsPinActual
            };
        }

        public CommentNotifierDataModel GetCommentNotifierDataModel(IPublishedContent content, CommentModel comment, Enum notificationType, Guid notifierId)
        {
            return new CommentNotifierDataModel
            {
                CommentId = comment.Id,
                NotificationType = notificationType,
                NotifierId = notifierId,
                Title = content.Name,
                Url = _commentLinkHelper.GetDetailsUrlWithComment(content, comment.Id),
                IsPinned = false,
                IsPinActual = false
            };
        }

        public ActivityNotifierDataModel GetActivityNotifierDataModel(IIntranetActivity activity, Enum notificationType, Guid notifierId)
        {
            return new ActivityNotifierDataModel
            {
                NotificationType = notificationType,
                ActivityType = activity.Type,
                Title = GetNotifierDataTitle(activity).TrimByWordEnd(MaxTitleLength),
                Url = _linkService.GetLinks(activity.Id).Details,
                NotifierId = notifierId,
                IsPinned = activity.IsPinned,
                IsPinActual = activity.IsPinActual
            };
        }

        public LikesNotifierDataModel GetLikesNotifierDataModel(IIntranetActivity activity, Enum notificationType, Guid notifierId)
        {
            return new LikesNotifierDataModel
            {
                Title = GetNotifierDataTitle(activity).TrimByWordEnd(MaxTitleLength),
                NotificationType = notificationType,
                ActivityType = activity.Type,
                NotifierId = notifierId,
                CreatedDate = DateTime.UtcNow,
                Url = _linkService.GetLinks(activity.Id).Details,
                IsPinned = activity.IsPinned,
                IsPinActual = activity.IsPinActual
            };
        }

        public ActivityReminderDataModel GetActivityReminderDataModel(IIntranetActivity activity, Enum activityType)
        {
            var model = new ActivityReminderDataModel
            {
                Url = _linkService.GetLinks(activity.Id).Details,
                Title = activity.Title,
                NotificationType = activityType,
                ActivityType = activity.Type,
                IsPinned = activity.IsPinned,
                IsPinActual = activity.IsPinActual
            };

            if (activity.Type is IntranetActivityTypeEnum.Events)
            {
                var @event = (Event)activity;
                model.StartDate = @event.StartDate;
            }

            return model;
        }

        public GroupInvitationDataModel GetGroupInvitationDataModel(
            NotificationTypeEnum notificationType,
            Guid groupId,
            Guid notifierId,
            Guid receiverId) =>
            new GroupInvitationDataModel
            {
                Url = $"/groups/room?groupId={groupId}",
                Title = _groupService.Get(groupId).Title,
                NotificationType = notificationType,
                GroupId = groupId,
                NotifierId = notifierId,
                ReceiverId = receiverId
            };

        private static string GetNotifierDataTitle(IIntranetActivity activity)
            => activity.Type is IntranetActivityTypeEnum.Bulletins ? activity.Description.StripHtml() : activity.Title;
    }
}