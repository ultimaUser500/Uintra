﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compent.Uintra.Core.Activity;
using Compent.Uintra.Core.Feed;
using Uintra.CentralFeed;
using Uintra.CentralFeed.Web;
using Uintra.Core;
using Uintra.Core.Activity;
using Uintra.Core.Feed;
using Uintra.Core.Permissions.Interfaces;
using Uintra.Core.Permissions.TypeProviders;
using Uintra.Core.User;
using Uintra.Groups;

namespace Compent.Uintra.Controllers
{
    public class CentralFeedController : CentralFeedControllerBase
    {        
        private readonly IIntranetMemberService<IGroupMember> _intranetMemberService;
        private readonly IGroupFeedService _groupFeedService;
        private readonly IFeedActivityHelper _feedActivityHelper;

        public CentralFeedController(
            ICentralFeedService centralFeedService,
            ICentralFeedContentService centralFeedContentService,
            IActivitiesServiceFactory activitiesServiceFactory,
            IIntranetMemberService<IGroupMember> intranetMemberService,
            IFeedTypeProvider centralFeedTypeProvider,
            IFeedLinkService feedLinkService,
            IGroupFeedService groupFeedService,
            IFeedActivityHelper feedActivityHelper,
            IFeedFilterStateService<FeedFiltersState> feedFilterStateService,
            IPermissionsService permissionsService,
            IContextTypeProvider contextTypeProvider,
            IFeedFilterService feedFilterService,
            IPermissionResourceTypeProvider permissionResourceTypeProvider)
            : base(
                  centralFeedService,
                  centralFeedContentService,
                  activitiesServiceFactory,
                  centralFeedTypeProvider,
                  feedLinkService,
                  feedFilterStateService,
                  permissionsService,
                  contextTypeProvider,
                  feedFilterService,
                  permissionResourceTypeProvider)
        {
            _intranetMemberService = intranetMemberService;
            _groupFeedService = groupFeedService;
            _feedActivityHelper = feedActivityHelper;
        }

        protected override IEnumerable<IFeedItem> GetCentralFeedItems(Enum type)
        {
            var groupIds = _intranetMemberService.GetCurrentMember().GroupIds;

            var groupFeed = IsTypeForAllActivities(type)
                ? _groupFeedService.GetFeed(groupIds)
                : _groupFeedService.GetFeed(type, groupIds);

            return base.GetCentralFeedItems(type)
                .Concat(groupFeed)
                .OrderByDescending(item => item.PublishDate);
        }

        protected override ActivityFeedOptions GetActivityFeedOptions(Guid activityId)
        {
            var options = base.GetActivityFeedOptions(activityId);

            return new ActivityFeedOptionsWithGroups
            {
                Links = options.Links,
                IsReadOnly = options.IsReadOnly,
                GroupInfo = _feedActivityHelper.GetGroupInfo(activityId)
            };
        }
    }
}