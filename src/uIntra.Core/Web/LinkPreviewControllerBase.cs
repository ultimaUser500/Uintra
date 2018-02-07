﻿using System.Threading.Tasks;
using System.Web.Http;
using Compent.LinkPreview.Client;
using uIntra.Core.Extensions;
using uIntra.Core.LinkPreview;
using uIntra.Core.LinkPreview.Sql;
using uIntra.Core.Persistence;
using Umbraco.Web.WebApi;

namespace uIntra.Core.Web
{
    public abstract class LinkPreviewControllerBase : UmbracoApiController
    {
        private readonly ILinkPreviewService _linkPreviewService;
        private readonly ILinkPreviewConfigProvider _configProvider;
        private readonly ISqlRepository<int, LinkPreviewEntity> _previewRepository;
        private readonly LinkPreviewModelMapper _linkPreviewModelMapper;

        protected LinkPreviewControllerBase(ILinkPreviewService linkPreviewService,
            ILinkPreviewConfigProvider configProvider,
            ISqlRepository<int, LinkPreviewEntity> previewRepository, LinkPreviewModelMapper linkPreviewModelMapper)
        {
            _linkPreviewService = linkPreviewService;
            _configProvider = configProvider;
            _previewRepository = previewRepository;
            _linkPreviewModelMapper = linkPreviewModelMapper;
        }

        [HttpGet]
        public async Task<LinkPreview.LinkPreview> Preview(string url)
        {
            var result = await _linkPreviewService.GetLinkPreview(url);
            var entity = Map(result, url);
            _previewRepository.Add(entity);

            var model = _linkPreviewModelMapper.MapPreview(entity);
            return model;
        }

        private LinkPreviewEntity Map(Compent.LinkPreview.Client.LinkPreview model, string url)
        {
            var entity = model.Map<LinkPreviewEntity>();
            entity.Uri = url;
            return entity;
        }

        [HttpGet]
        public LinkDetectionConfig Config()
        {
            return _configProvider.Config;
        }
    }
}