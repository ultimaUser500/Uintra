﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using uIntra.Core.Activity;
using uIntra.Core.Controls.LightboxGallery;
using uIntra.Core.Extentions;
using uIntra.Core.Grid;
using uIntra.Core.Links;
using uIntra.Core.Media;
using uIntra.Core.TypeProviders;
using uIntra.Core.User;
using uIntra.Core.User.Permissions.Web;
using uIntra.Events.Core.Models;
using Umbraco.Web.Mvc;

namespace uIntra.Events.Web
{
    [ActivityController(ActivityTypeId)]
    public abstract class EventsControllerBase : SurfaceController
    {
        protected virtual string ComingEventsViewPath => "~/App_Plugins/Events/ComingEvents/ComingEventsView.cshtml";
        protected virtual string DetailsViewPath => "~/App_Plugins/Events/Details/DetailsView.cshtml";
        protected virtual string CreateViewPath => "~/App_Plugins/Events/Create/CreateView.cshtml";
        protected virtual string EditViewPath => "~/App_Plugins/Events/Edit/EditView.cshtml";
        protected virtual string ItemViewPath => "~/App_Plugins/Events/List/ItemView.cshtml";
        protected virtual string PreviewItemViewPath => "~/App_Plugins/Events/PreviewItem/PreviewItemView.cshtml";
        protected virtual int DisplayedImagesCount { get; } = 3;

        private readonly IEventsService<EventBase> _eventsService;
        private readonly IMediaHelper _mediaHelper;
        private readonly IIntranetUserService<IIntranetUser> _intranetUserService;
        private readonly IActivityTypeProvider _activityTypeProvider;
        private readonly IActivityLinkService _activityLinkService;

        private const int ActivityTypeId = (int)IntranetActivityTypeEnum.Events;

        protected EventsControllerBase(
            IEventsService<EventBase> eventsService,
            IMediaHelper mediaHelper,
            IIntranetUserService<IIntranetUser> intranetUserService,
            IActivityTypeProvider activityTypeProvider,
            IActivityLinkService activityLinkService)
        {
            _eventsService = eventsService;
            _mediaHelper = mediaHelper;
            _intranetUserService = intranetUserService;
            _activityTypeProvider = activityTypeProvider;
            _activityLinkService = activityLinkService;
        }

        public virtual ActionResult Details(Guid id, ActivityLinks links)
        {
            var @event = _eventsService.Get(id);
            if (@event.IsHidden)
            {
                HttpContext.Response.Redirect(links.Overview);
            }

            var model = GetViewModel(@event, links);

            return PartialView(DetailsViewPath, model);
        }

        [HttpGet]
        public virtual ActionResult ComingEvents(ComingEventsPanelModel panelModel)
        {
            var viewModel = GetComingEventsViewModel(panelModel);
            return PartialView(ComingEventsViewPath, viewModel);
        }

        protected virtual ComingEventsPanelViewModel GetComingEventsViewModel(ComingEventsPanelModel panelModel)
        {
            var comingEvents = GetComingEvents(panelModel.EventsAmount);
            var viewModel = new ComingEventsPanelViewModel()
            {
                OverviewUrl = comingEvents.FirstOrDefault()?.Links.Overview,
                Title = panelModel.DisplayTitle,
                Events = comingEvents
            };
            return viewModel;
        }

        protected virtual IList<ComingEventViewModel> GetComingEvents(int eventsAmount)
        {
            var events = _eventsService.GetComingEvents(DateTime.UtcNow).Take(eventsAmount);
            var eventsList = events as IList<EventBase> ?? events.ToList();

            var creatorsDictionary = _intranetUserService.GetMany(eventsList.Select(e => e.CreatorId)).ToDictionary(c => c.Id);

            var comingEvents = new List<ComingEventViewModel>();

            foreach (var e in eventsList)
            {
                var viewModel = e.Map<ComingEventViewModel>();
                viewModel.Creator = creatorsDictionary[e.CreatorId];
                viewModel.Links = _activityLinkService.GetLinks(e.Id);
                comingEvents.Add(viewModel);
            }

            return comingEvents;
        }

        [RestrictedAction(ActivityTypeId, IntranetActivityActionEnum.Create)]
        public virtual ActionResult Create(ActivityCreateLinks links)
        {
            var model = GetCreateModel(links);
            return PartialView(CreateViewPath, model);
        }

        [HttpPost]
        [RestrictedAction(ActivityTypeId, IntranetActivityActionEnum.Create)]
        public virtual ActionResult Create(EventCreateModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToCurrentUmbracoPage(Request.QueryString);
            }

            var @event = MapToEvent(createModel);
            var activityId = _eventsService.Create(@event);
            OnEventCreated(activityId, createModel);

            var redirectUrl = createModel.Links.DetailsNoId.AddIdParameter(activityId);
            return Redirect(redirectUrl);
        }

        [RestrictedAction(ActivityTypeId, IntranetActivityActionEnum.Edit)]
        public virtual ActionResult Edit(Guid id, ActivityLinks links)
        {

            var @event = _eventsService.Get(id);
            if (@event.IsHidden)
            {
                HttpContext.Response.Redirect(ViewData.GetActivityOverviewPageUrl(ActivityTypeId));
            }

            var model = GetEditViewModel(@event, links);
            return PartialView(EditViewPath, model);
        }

        [HttpPost]
        [RestrictedAction(ActivityTypeId, IntranetActivityActionEnum.Edit)]
        public virtual ActionResult Edit(EventEditModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToCurrentUmbracoPage(Request.QueryString);
            }

            var cachedActivityMedias = _eventsService.Get(editModel.Id).MediaIds;

            var activity = MapToEvent(editModel);
            _eventsService.Save(activity);

            DeleteMedia(cachedActivityMedias.Except(activity.MediaIds));

            OnEventEdited(activity, editModel);

            return Redirect(editModel.Links.Details);
        }

        [HttpPost]
        public virtual void Hide(Guid id, bool isNotificationNeeded)
        {
            _eventsService.Hide(id);
            OnEventHidden(id, isNotificationNeeded);
        }

        [HttpPost]
        public virtual JsonResult HasConfirmation(EventEditModel model)
        {
            var @event = MapEditModel(model);
            return Json(new { HasConfirmation = _eventsService.IsActual(@event) });
        }

        protected virtual EventCreateModel GetCreateModel(IActivityCreateLinks links)
        {
            var mediaSettings = _eventsService.GetMediaSettings();
            var model = new EventCreateModel
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(8),
                CanSubscribe = true,
                Creator = _intranetUserService.GetCurrentUser(),
                ActivityType = _activityTypeProvider.Get(ActivityTypeId),
                Links = links,
                MediaRootId = mediaSettings.MediaRootId
            };
            return model;
        }

        protected virtual EventPreviewViewModel GetPreviewViewModel(EventBase @event, ActivityLinks links)
        {
            var creator = _intranetUserService.Get(@event);
            return new EventPreviewViewModel()
            {
                Id = @event.Id,
                Title = @event.Title,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate,
                Creator = creator,
                ActivityType = @event.Type,
                Links = links                
            };
        }

        protected virtual IEnumerable<EventBase> GetComingEvents(DateTime fromDate)
        {
            return _eventsService.GetComingEvents(fromDate);
        }

        protected virtual EventBase MapEditModel(EventEditModel saveModel)
        {
            var @event = _eventsService.Get(saveModel.Id);
            @event = Mapper.Map(saveModel, @event);
            return @event;
        }

        protected virtual EventEditModel GetEditViewModel(EventBase @event, ActivityLinks links)
        {
            var model = @event.Map<EventEditModel>();
            var mediaSettings = _eventsService.GetMediaSettings();
            model.MediaRootId = mediaSettings.MediaRootId;
            FillMediaSettingsData(mediaSettings);

            model.CanEditSubscribe = _eventsService.CanEditSubscribe(@event.Id);
            model.Creator = _intranetUserService.Get(@event);

            model.Links = links;
            return model;
        }

        protected virtual void FillMediaSettingsData(MediaSettings settings)
        {
            ViewData["AllowedMediaExtentions"] = settings.AllowedMediaExtentions;
        }

        protected virtual EventViewModel GetViewModel(EventBase @event, ActivityLinks links)
        {
            var model = @event.Map<EventViewModel>();

            model.CanEdit = _eventsService.CanEdit(@event);
            model.CanSubscribe = _eventsService.CanSubscribe(@event);
            model.Links = links;

            model.HeaderInfo = @event.Map<IntranetActivityDetailsHeaderViewModel>();
            model.HeaderInfo.Dates = new[] { @event.StartDate.ToDateTimeFormat(), @event.EndDate.ToDateTimeFormat() };
            model.HeaderInfo.Creator = _intranetUserService.Get(@event);
            model.HeaderInfo.Links = links;

            return model;
        }

        protected virtual EventItemViewModel GetItemViewModel(EventBase @event, IActivityLinks links)
        {
            var model = @event.Map<EventItemViewModel>();

            model.MediaIds = @event.MediaIds;
            model.CanSubscribe = _eventsService.CanSubscribe(@event);
            model.LightboxGalleryPreviewInfo = GetGalleryPreviewInfo(@event);
            model.Links = links;

            model.HeaderInfo = @event.Map<IntranetActivityItemHeaderViewModel>();
            model.HeaderInfo.Creator = _intranetUserService.Get(@event);
            model.HeaderInfo.Links = links;

            return model;
        }

        private LightboxGalleryPreviewModel GetGalleryPreviewInfo(EventBase @event)
        {
            return new LightboxGalleryPreviewModel
            {
                MediaIds = @event.MediaIds,
                DisplayedImagesCount = DisplayedImagesCount,
                ActivityId = @event.Id,
                ActivityType = @event.Type,
            };
        }

        protected virtual EventBase MapToEvent(EventCreateModel createModel)
        {
            var @event = createModel.Map<EventBase>();

            @event.MediaIds = @event.MediaIds.Concat(_mediaHelper.CreateMedia(createModel));
            @event.StartDate = createModel.StartDate.ToUniversalTime();
            @event.EndDate = createModel.EndDate.ToUniversalTime();
            @event.EndPinDate = createModel.EndPinDate?.ToUniversalTime();

            return @event;
        }

        protected virtual EventBase MapToEvent(EventEditModel editModel)
        {
            var @event = MapEditModel(editModel);

            @event.MediaIds = @event.MediaIds.Concat(_mediaHelper.CreateMedia(editModel));
            @event.UmbracoCreatorId = _intranetUserService.Get(editModel.CreatorId).UmbracoId;
            @event.StartDate = editModel.StartDate.ToUniversalTime();
            @event.EndDate = editModel.EndDate.ToUniversalTime();
            @event.EndPinDate = editModel.EndPinDate?.ToUniversalTime();

            if (_eventsService.CanEditSubscribe(@event.Id))
            {
                @event.CanSubscribe = editModel.CanSubscribe;
            }

            return @event;
        }

        protected virtual void DeleteMedia(IEnumerable<int> mediaIds)
        {
            _mediaHelper.DeleteMedia(mediaIds);
        }

        protected virtual void OnEventCreated(Guid activityId, EventCreateModel model)
        {
        }

        protected virtual void OnEventEdited(EventBase @event, EventEditModel model)
        {
        }

        protected virtual void OnEventHidden(Guid id, bool isNotificationNeeded)
        {
        }
    }
}