﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Compent.uIntra.Core.UmbracoModelsBuilders;
using uIntra.CentralFeed;
using uIntra.Core;
using uIntra.Core.Activity;
using uIntra.Core.Extentions;
using uIntra.Core.Grid;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Compent.uIntra.Core.CentralFeed
{
    public class CentralFeedContentHelper : ICentralFeedContentHelper
    {
        private const string CentralFeedFiltersStateCookieName = "centralFeedFiltersState";
        private readonly UmbracoHelper _umbracoHelper;
        private readonly ICentralFeedService _centralFeedService;
        private readonly IGridHelper _gridHelper;

        public CentralFeedContentHelper(
            UmbracoHelper umbracoHelper,
            ICentralFeedService centralFeedService,
            IGridHelper gridHelper)
        {
            _umbracoHelper = umbracoHelper;
            _centralFeedService = centralFeedService;
            _gridHelper = gridHelper;
        }

        public IPublishedContent GetOverviewPage()
        {
            return _umbracoHelper.TypedContentSingleAtXPath(XPathHelper.GetXpath(HomePage.ModelTypeAlias));
        }

        public bool IsCentralFeedPage(IPublishedContent currentPage)
        {
            return GetOverviewPage().Id == currentPage.Id || GetContents().Any(c => c.IsAncestorOrSelf(currentPage));
        }

        public IEnumerable<CentralFeedTabModel> GetTabs(IPublishedContent currentPage)
        {
            var overviewPage = GetOverviewPage();
            yield return new CentralFeedTabModel
            {
                Content = overviewPage,
                Type = GetTabType(overviewPage),
                IsActive = overviewPage.Id == currentPage.Id
            };

            foreach (var content in GetContents())
            {
                var tabType = GetTabType(content);
                var activityType = tabType.GetHashCode().ToEnum<IntranetActivityTypeEnum>();

                if (activityType == null)
                {
                    continue;
                }

                var settings = _centralFeedService.GetSettings(tabType);
                yield return new CentralFeedTabModel
                {
                    Content = content,
                    Type = tabType,
                    HasSubscribersFilter = settings.HasSubscribersFilter,
                    HasBulletinFilter = settings.HasBulletinFilter,
                    HasPinnedFilter = settings.HasPinnedFilter,
                    CreateUrl = content.Children.SingleOrDefault(n => n.DocumentTypeAlias.In(NewsCreatePage.ModelTypeAlias, EventsCreatePage.ModelTypeAlias))?.Url,
                    IsActive = content.IsAncestorOrSelf(currentPage)
                };
            }
        }

        public void SaveFiltersState(CentralFeedFiltersStateModel stateModel)
        {
            var cookie = HttpContext.Current.Request.Cookies[CentralFeedFiltersStateCookieName];
            cookie.Value = stateModel.ToJson();
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public CentralFeedFiltersStateModel GetFiltersState()
        {
            var cookie = HttpContext.Current.Request.Cookies[CentralFeedFiltersStateCookieName];
            if (string.IsNullOrEmpty(cookie?.Value))
            {
                cookie = new HttpCookie(CentralFeedFiltersStateCookieName)
                {
                    Expires = DateTime.Now.AddDays(7),
                    Value = GetDefaultCentralFeedFiltersState().ToJson()
                };

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            return cookie.Value.Deserialize<CentralFeedFiltersStateModel>();
        }

        public bool CentralFeedCookieExists()
        {
            var cookie= HttpContext.Current.Request.Cookies[CentralFeedFiltersStateCookieName];

            return cookie != null && cookie.Value.IsNotNullOrEmpty();
        }

        public void ClearFiltersState()
        {
            var cookie = HttpContext.Current.Request.Cookies[CentralFeedFiltersStateCookieName];
            cookie.Expires = DateTime.Now.AddDays(-1);
        }

        public CentralFeedTypeEnum GetTabType(IPublishedContent content)
        {
            var value = _gridHelper.GetValue(content, "custom.CentralFeed");

            if (value == null || value.tabType == null)
            {
                return default(CentralFeedTypeEnum);
            }

            int tabType;
            if (int.TryParse(value.tabType.ToString(), out tabType))
            {
                return (CentralFeedTypeEnum)tabType;
            }
            return default(CentralFeedTypeEnum);
        }

        private IEnumerable<IPublishedContent> GetContents()
        {
            return GetOverviewPage().Children.Where(c => c.DocumentTypeAlias.In(NewsOverviewPage.ModelTypeAlias, EventsOverviewPage.ModelTypeAlias));
        }

        private CentralFeedFiltersStateModel GetDefaultCentralFeedFiltersState()
        {
            return new CentralFeedFiltersStateModel()
            {
                BulletinFilterSelected = true
            };
        }
    }
}