﻿using Compent.Uintra.Core.Helpers;
using System;
using Uintra.Comments;
using Uintra.Core.Activity;
using Uintra.Core.User;
using Uintra.Notification;
using Uintra.Notification.Base;
using Uintra.Notification.Configuration;
using Umbraco.Web;
using static LanguageExt.Prelude;

namespace Compent.Uintra.Core.PagePromotion
{
    public class PagePromotionNotificationService : INotifyableService
    {
        private readonly INotificationsService _notificationsService;
        private readonly INotifierDataHelper _notifierDataHelper;
        private readonly ICommentsService _commentsService;
        private readonly IIntranetMemberService<IIntranetMember> _memberService;
        private readonly UmbracoHelper _umbracoHelper;

        public PagePromotionNotificationService(INotificationsService notificationsService,
            INotifierDataHelper notifierDataHelper,
            ICommentsService commentsService,
            IIntranetMemberService<IIntranetMember> memberService,
            UmbracoHelper umbracoHelper)
        {
            _notificationsService = notificationsService;
            _notifierDataHelper = notifierDataHelper;
            _commentsService = commentsService;
            _memberService = memberService;
            _umbracoHelper = umbracoHelper;
        }

        public Enum Type => IntranetActivityTypeEnum.PagePromotion;

        public void Notify(Guid entityId, Enum notificationType)
        {
            var notifierData = GetNotifierData(entityId, notificationType);
            if (notifierData != null)
            {
                _notificationsService.ProcessNotification(notifierData);
            }
        }

        private NotifierData GetNotifierData(Guid entityId, Enum notificationType)
        {
            var currentMember = _memberService.GetCurrentMember();

            var data = new NotifierData()
            {
                NotificationType = notificationType,
                ActivityType = Type,
            };

            switch (notificationType)
            {
                case NotificationTypeEnum.CommentLikeAdded:
                case NotificationTypeEnum.CommentReplied:
                    {
                        var comment = _commentsService.Get(entityId);
                        data.ReceiverIds = List(comment.UserId);
                        var currentContentPage = _umbracoHelper.TypedContent(comment.ActivityId);
                        data.Value = _notifierDataHelper.GetCommentNotifierDataModel(currentContentPage, comment, notificationType, currentMember.Id);
                    }
                    break;
                default: return null;
            }
            return data;
        }
    }
}