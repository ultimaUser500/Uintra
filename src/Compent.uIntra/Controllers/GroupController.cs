﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Compent.CommandBus;
using Uintra.Core;
using Uintra.Core.Links;
using Uintra.Core.Media;
using Uintra.Core.User;
using Uintra.Groups;
using Uintra.Groups.Navigation.Models;
using Uintra.Groups.Web;
using Uintra.Navigation;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Compent.Uintra.Controllers
{
    public class GroupController : GroupControllerBase
    {
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IDocumentTypeAliasProvider _documentTypeAliasProvider;
        private readonly IGroupService _groupService;

        public GroupController(
            IGroupService groupService,
            IGroupMemberService groupMemberService,
            IMediaHelper mediaHelper,
            IGroupLinkProvider groupLinkProvider,
            IGroupMediaService groupMediaService,
            IIntranetMemberService<IGroupMember> intranetMemberService,
            IProfileLinkProvider profileLinkProvider,
            UmbracoHelper umbracoHelper,
            IDocumentTypeAliasProvider documentTypeAliasProvider,
            IImageHelper imageHelper,
            ICommandPublisher commandPublisher)
            : base(
                groupService,
                groupMemberService,
                mediaHelper,
                groupMediaService,
                intranetMemberService,
                profileLinkProvider,
                groupLinkProvider,
                imageHelper,
                commandPublisher)
        {
            _umbracoHelper = umbracoHelper;
            _documentTypeAliasProvider = documentTypeAliasProvider;
            _groupService = groupService;
        }

        public override ActionResult LeftNavigation()
        {
            var groupPageXpath = XPathHelper.GetXpath(_documentTypeAliasProvider.GetHomePage(), _documentTypeAliasProvider.GetGroupOverviewPage());
            var groupPage = _umbracoHelper.TypedContentSingleAtXPath(groupPageXpath);

            var isPageActive = GetIsPageActiveFunc(_umbracoHelper.AssignedContentItem);

            var menuItems = GetMenuItems(groupPage);

            var result = new GroupLeftNavigationMenuViewModel
            {
                Items = menuItems,
                GroupOverviewPageUrl = groupPage.Url,
                IsActive = isPageActive(groupPage)
            };

            return PartialView(LeftNavigationPath, result);
        }
         
        private IEnumerable<GroupLeftNavigationItemViewModel> GetMenuItems(IPublishedContent rootGroupPage)
        {
            var isPageActive = GetIsPageActiveFunc(_umbracoHelper.AssignedContentItem);

            var groupPageChildren = rootGroupPage.Children.Where(el => el.IsShowPageInSubNavigation()).ToList();

            foreach (var subPage in groupPageChildren)
            {
                if (subPage.IsShowPageInSubNavigation())
                {
                    if (_groupService.ValidatePermission(subPage))
                    {
                        yield return MapToLeftNavigationItem(subPage, isPageActive);
                    }
                }
                else
                {
                    yield return MapToLeftNavigationItem(subPage, isPageActive);
                }
            }
        }

        private static Func<IPublishedContent, bool> GetIsPageActiveFunc(IPublishedContent currentPage)
        {
            return p => currentPage.Id == p.Id;
        }

        private static GroupLeftNavigationItemViewModel MapToLeftNavigationItem(IPublishedContent page, Func<IPublishedContent, bool> isPageActive)
        {
            return new GroupLeftNavigationItemViewModel
            {
                Name = page.GetNavigationName(),
                Url = page.Url,
                IsActive = isPageActive(page)
            };
        }
    }
}