﻿using Uintra.CentralFeed;
using Uintra.Core.Activity;
using Uintra.Core.Extensions;
using Uintra.Core.Links;

namespace Uintra.Groups
{
    public class GroupFeedLinkProvider : FeedLinkProvider, IGroupFeedLinkProvider
    {
        public GroupFeedLinkProvider(
            IActivityPageHelperFactory pageHelperFactory,
            IProfileLinkProvider profileLinkProvider) 
            : base(pageHelperFactory, profileLinkProvider)
        {
        }

        public IActivityLinks GetLinks(GroupActivityTransferModel activity)
        {
            var helper = GetPageHelper(activity.Type);

            return new ActivityLinks
            {
                Feed = helper.GetFeedUrl()?.AddGroupId(activity.GroupId),
                Overview = helper.GetOverviewPageUrl().AddGroupId(activity.GroupId),
                Create = helper.GetCreatePageUrl()?.AddGroupId(activity.GroupId),
                Details = helper.GetDetailsPageUrl(activity.Id).AddGroupId(activity.GroupId),
                Edit = helper.GetEditPageUrl(activity.Id).AddGroupId(activity.GroupId),
                Owner = GetProfileLink(activity.OwnerId),
                DetailsNoId = helper.GetDetailsPageUrl().AddGroupId(activity.GroupId)
            };
        }

        public IActivityCreateLinks GetCreateLinks(GroupActivityTransferCreateModel model)
        {
            var helper = GetPageHelper(model.Type);

            return new ActivityCreateLinks
            {
                Feed = helper.GetFeedUrl()?.AddGroupId(model.GroupId),
                Overview = helper.GetOverviewPageUrl().AddGroupId(model.GroupId),
                Create = helper.GetCreatePageUrl()?.AddGroupId(model.GroupId),
                Owner = GetProfileLink(model.OwnerId),
                DetailsNoId = helper.GetDetailsPageUrl().AddGroupId(model.GroupId)
            };
        }
    }
}
