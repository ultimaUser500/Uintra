﻿using System.Collections.Generic;
using System.Linq;
using uIntra.Core;
using uIntra.Core.Extensions;
using uIntra.Core.TypeProviders;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace uIntra.CentralFeed.Providers
{
    public class CentralFeedContentProvider : FeedContentProviderBase, ICentralFeedContentProvider
    {
        protected override IEnumerable<string> OverviewXPath { get; }

        private readonly IDocumentTypeAliasProvider _documentTypeAliasProvider;
        private readonly IActivityTypeProvider _activityTypeProvider;

        public CentralFeedContentProvider(IDocumentTypeAliasProvider documentTypeAliasProvider,
            UmbracoHelper umbracoHelper, IActivityTypeProvider activityTypeProvider)
            : base(umbracoHelper)
        {
            _documentTypeAliasProvider = documentTypeAliasProvider;
            _activityTypeProvider = activityTypeProvider;

            OverviewXPath = documentTypeAliasProvider.GetHomePage().ToEnumerableOfOne();
        }

        public override IEnumerable<IPublishedContent> GetRelatedPages()
        {
            var activityAliases = _activityTypeProvider
                .GetAll()
                .Select(_documentTypeAliasProvider.GetOverviewPage)
                .ToArray();
            return GetOverviewPage().Children.Where(c => c.DocumentTypeAlias.In(activityAliases));
        }
    }
}