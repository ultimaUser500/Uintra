﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compent.CommandBus;
using Compent.Extensions;
using Compent.Uintra.Core.Notification;
using Compent.Uintra.Core.Search.Entities;
using Compent.Uintra.Core.UserTags.Indexers;
using Uintra.CentralFeed;
using Uintra.Comments;
using Uintra.Core.Activity;
using Uintra.Core.Caching;
using Uintra.Core.Extensions;
using Uintra.Core.LinkPreview;
using Uintra.Core.Links;
using Uintra.Core.Location;
using Uintra.Core.Media;
using Uintra.Core.Permissions;
using Uintra.Core.Permissions.Interfaces;
using Uintra.Core.TypeProviders;
using Uintra.Core.User;
using Uintra.Groups;
using Uintra.Likes;
using Uintra.News;
using Uintra.Notification;
using Uintra.Notification.Base;
using Uintra.Search;
using Uintra.Tagging.UserTags;
using static Uintra.Notification.Configuration.NotificationTypeEnum;

namespace Compent.Uintra.Core.News
{
    public class NewsService : NewsServiceBase<Entities.News>,
        INewsService<Entities.News>,
        IFeedItemService,
        INotifyableService,
        IIndexer,
        IHandle<VideoConvertedCommand>
    {
        private readonly ICommentsService _commentsService;
        private readonly ILikesService _likesService;
        private readonly INotificationsService _notificationService;
        private readonly IMediaHelper _mediaHelper;
        private readonly IElasticUintraActivityIndex _activityIndex;
        private readonly IDocumentIndexer _documentIndexer;
        private readonly IIntranetMediaService _intranetMediaService;
        private readonly IGroupActivityService _groupActivityService;
        private readonly IActivityLinkService _linkService;
        private readonly INotifierDataBuilder _notifierDataBuilder;
        private readonly IUserTagService _userTagService;
        private readonly IActivityLocationService _activityLocationService;
        private readonly IGroupService _groupService;

        public NewsService(IIntranetActivityRepository intranetActivityRepository,
            ICacheService cacheService,
            IIntranetMemberService<IIntranetMember> intranetMemberService,
            ICommentsService commentsService,
            ILikesService likesService,
            IPermissionsService permissionsService,
            INotificationsService notificationService,
            IMediaHelper mediaHelper,
            IElasticUintraActivityIndex activityIndex,
            IDocumentIndexer documentIndexer,
            IActivityTypeProvider activityTypeProvider,
            IIntranetMediaService intranetMediaService,
            IGroupActivityService groupActivityService,
            IActivityLinkService linkService,
            IActivityLocationService activityLocationService,
            IUserTagService userTagService,
            IActivityLinkPreviewService activityLinkPreviewService,
            IGroupService groupService,
            INotifierDataBuilder notifierDataBuilder)
            : base(intranetActivityRepository, cacheService, intranetMemberService, 
                activityTypeProvider, intranetMediaService, activityLocationService, activityLinkPreviewService,
                permissionsService)
        {
            _commentsService = commentsService;
            _likesService = likesService;
            _notificationService = notificationService;
            _mediaHelper = mediaHelper;
            _activityIndex = activityIndex;
            _documentIndexer = documentIndexer;
            _intranetMediaService = intranetMediaService;
            _groupActivityService = groupActivityService;
            _linkService = linkService;
            _userTagService = userTagService;
            _groupService = groupService;
            _notifierDataBuilder = notifierDataBuilder;
            _activityLocationService = activityLocationService;
        }

        public override Enum Type => IntranetActivityTypeEnum.News;

        public override Enum PermissionActivityType => PermissionResourceTypeEnum.News;

        public MediaSettings GetMediaSettings()
        {
            return _mediaHelper.GetMediaFolderSettings(MediaFolderTypeEnum.NewsContent);
        }

        public FeedSettings GetFeedSettings() =>
            new FeedSettings
            {
                Type = CentralFeedTypeEnum.News,
                Controller = "News",
                HasSubscribersFilter = false,
                HasPinnedFilter = true,
            };

        public IEnumerable<IFeedItem> GetItems()
        {
            var items = GetOrderedActualItems();
            return items;
        }

        private IOrderedEnumerable<Entities.News> GetOrderedActualItems()
        {
            var items = GetManyActual().OrderByDescending(i => i.PublishDate);
            return items;
        }

        protected override void MapBeforeCache(IList<Entities.News> cached)
        {
            foreach (var activity in cached)
            {
                var entity = activity;
                entity.Location = _activityLocationService.Get(entity.Id);
                entity.GroupId = _groupActivityService.GetGroupId(activity.Id);
                _commentsService.FillComments(entity);
                _likesService.FillLikes(entity);
            }
        }

        protected override void UpdateCache()
        {
            base.UpdateCache();
            FillIndex();
        }

        public override Entities.News UpdateActivityCache(Guid id)
        {
            var cachedNews = Get(id);
            var news = base.UpdateActivityCache(id);
            if (IsInCache(news) && (news.GroupId is null || _groupService.IsActivityFromActiveGroup(news)))
            {
                _activityIndex.Index(Map(news));
                _documentIndexer.Index(news.MediaIds);
                return news;
            }

            if (cachedNews == null) return null;

            _activityIndex.Delete(id);
            _documentIndexer.DeleteFromIndex(cachedNews.MediaIds);
            _mediaHelper.DeleteMedia(cachedNews.MediaIds);
            return null;
        }

        public void Notify(Guid entityId, Enum notificationType)
        {
            NotifierData notifierData;

            if (notificationType.In(CommentAdded, CommentEdited, CommentLikeAdded, CommentReplied))
            {
                var comment = _commentsService.Get(entityId);
                var parentActivity = Get(comment.ActivityId);
                notifierData = _notifierDataBuilder.GetNotifierData(comment, parentActivity, notificationType);
            }
            else
            {
                var activity = Get(entityId);
                notifierData = _notifierDataBuilder.GetNotifierData(activity, notificationType);
            }

            _notificationService.ProcessNotification(notifierData);
        }

        public void FillIndex()
        {
            var activities = GetAll().Where(IsInCache);
            var searchableActivities = activities.Select(Map);
            _activityIndex.DeleteByType(UintraSearchableTypeEnum.News);
            _activityIndex.Index(searchableActivities);
        }

        private static bool IsInCache(Entities.News news)
        {
            return !IsNewsHidden(news) && IsActualPublishDate(news);
        }

        private static bool IsNewsHidden(IIntranetActivity news) => 
            news == null || news.IsHidden;

        private static bool IsActualPublishDate(INewsBase news) => 
            DateTime.Compare(news.PublishDate, DateTime.UtcNow) <= 0;

        private SearchableUintraActivity Map(Entities.News news)
        {
            var searchableActivity = news.Map<SearchableUintraActivity>();
            searchableActivity.Url = _linkService.GetLinks(news.Id).Details;
            searchableActivity.UserTagNames = _userTagService.Get(news.Id).Select(t => t.Text);
            return searchableActivity;
        }

        public BroadcastResult Handle(VideoConvertedCommand command)
        {
            var entityId = _intranetMediaService.GetEntityIdByMediaId(command.MediaId);
            var entity = Get(entityId);
            if (entity == null)
            {
                return BroadcastResult.Success;
            }

            entity.ModifyDate = DateTime.UtcNow;
            return BroadcastResult.Success;
        }
    }
}