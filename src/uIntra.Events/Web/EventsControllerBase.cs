﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Compent.Extensions;
using Uintra.Core;
using Uintra.Core.Activity;
using Uintra.Core.Attributes;
using Uintra.Core.Context;
using Uintra.Core.Controls.LightboxGallery;
using Uintra.Core.Extensions;
using Uintra.Core.Feed;
using Uintra.Core.Links;
using Uintra.Core.Media;
using Uintra.Core.Permissions;
using Uintra.Core.Permissions.Interfaces;
using Uintra.Core.TypeProviders;
using Uintra.Core.User;
using Uintra.Core.User.Permissions.Web;

namespace Uintra.Events.Web
{
    [ActivityController(ActivityTypeId)]
    public abstract class EventsControllerBase : ContextController
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
        private readonly IIntranetMemberService<IIntranetMember> _intranetMemberService;
        private readonly IActivityTypeProvider _activityTypeProvider;
        private readonly IActivityLinkService _activityLinkService;
        private readonly IActivityPageHelperFactory _activityPageHelperFactory;
        private readonly IPermissionsService _permissionsService;

        private const int ActivityTypeId = (int)IntranetActivityTypeEnum.Events;
        private const PermissionResourceTypeEnum ActivityType = PermissionResourceTypeEnum.Events;

        public override Enum ControllerContextType { get; } = ContextType.Events;

        protected EventsControllerBase(
            IEventsService<EventBase> eventsService,
            IMediaHelper mediaHelper,
            IIntranetMemberService<IIntranetMember> intranetMemberService,
            IActivityTypeProvider activityTypeProvider,
            IActivityLinkService activityLinkService,
            IContextTypeProvider contextTypeProvider,
            IActivityPageHelperFactory activityPageHelperFactory,
            IPermissionsService permissionsService) : base(contextTypeProvider)
        {
            _eventsService = eventsService;
            _mediaHelper = mediaHelper;
            _intranetMemberService = intranetMemberService;
            _activityTypeProvider = activityTypeProvider;
            _activityLinkService = activityLinkService;
            _activityPageHelperFactory = activityPageHelperFactory;
            _permissionsService = permissionsService;
        }

        [NotFoundActivity, RestrictedAction(ActivityType, PermissionActionEnum.View)]
        public virtual ActionResult Details(Guid id, ActivityFeedOptions options)
        {
            var @event = _eventsService.Get(id);
            var model = GetViewModel(@event, options);
            AddEntityIdentityForContext(id);
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
            (IList<ComingEventViewModel> comingEvents, int totalCount) = GetComingEvents(panelModel.EventsAmount);
            var viewModel = new ComingEventsPanelViewModel
            {
                OverviewUrl = _activityPageHelperFactory.GetHelper(IntranetActivityTypeEnum.Events).GetOverviewPageUrl(),
                Title = panelModel.DisplayTitle,
                Events = comingEvents,
                ShowSeeAllButton = comingEvents.Count < totalCount
            };
            return viewModel;
        }

        protected virtual (IList<ComingEventViewModel> events, int totalCount) GetComingEvents(int eventsAmount)
        {
            var events = GetComingEvents(DateTime.UtcNow).AsList();

            var ownersDictionary = _intranetMemberService.GetMany(events.Select(e => e.OwnerId)).ToDictionary(c => c.Id);

            var comingEvents = events
                .Take(eventsAmount)
                .Select(@event =>
                {
                    var viewModel = @event.Map<ComingEventViewModel>();
                    viewModel.Owner = ownersDictionary[@event.OwnerId].Map<MemberViewModel>();
                    viewModel.Links = _activityLinkService.GetLinks(@event.Id);
                    return viewModel;
                })
                .ToList();

            return (comingEvents, events.Count);
        }

        protected virtual IEnumerable<EventBase> GetComingEvents(DateTime startDate) => _eventsService.GetComingEvents(startDate);

        [RestrictedAction(ActivityType, PermissionActionEnum.Create)]
        public virtual ActionResult Create(ActivityCreateLinks links)
        {
            var model = GetCreateModel(links);
            return PartialView(CreateViewPath, model);
        }

        [HttpPost]
        [RestrictedAction(ActivityType, PermissionActionEnum.Create)]
        public virtual ActionResult Create(EventCreateModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToCurrentUmbracoPage(Request.QueryString);
            }

            var @event = MapToEvent(createModel);
            var activityId = _eventsService.Create(@event);
            OnEventCreated(activityId, createModel);

            var redirectUrl = _activityLinkService.GetLinks(activityId).Details;
            return Redirect(redirectUrl);
        }

        [RestrictedAction(ActivityType, PermissionActionEnum.Edit)]
        public virtual ActionResult Edit(Guid id, ActivityLinks links)
        {
            var @event = _eventsService.Get(id);
            if (@event.IsHidden)
            {
                HttpContext.Response.Redirect(links.Overview);
            }

            var model = GetEditViewModel(@event, links);
            return PartialView(EditViewPath, model);
        }

        [HttpPost]
        [RestrictedAction(ActivityType, PermissionActionEnum.Edit)]
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
            if (_eventsService.CanHide(id))
            {
                _eventsService.Hide(id);
                OnEventHidden(id, isNotificationNeeded);
            }
        }

        protected virtual bool IsPinAllowed()
        {
	        return _permissionsService.Check(ActivityType, PermissionActionEnum.CanPin);
        }

		public virtual JsonResult HasConfirmation(Guid id)
        {
            var @event = _eventsService.Get(id);
            return Json(new { HasConfirmation = _eventsService.IsActual(@event) }, JsonRequestBehavior.AllowGet);
        }

        protected virtual EventCreateModel GetCreateModel(IActivityCreateLinks links)
        {
            var mediaSettings = _eventsService.GetMediaSettings();
            var model = new EventCreateModel
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(8),
                PublishDate = DateTime.UtcNow,
                OwnerId = _intranetMemberService.GetCurrentMemberId(),
                ActivityType = _activityTypeProvider[ActivityTypeId],
                Links = links,
                MediaRootId = mediaSettings.MediaRootId,
				PinAllowed=IsPinAllowed()
            };
            return model;
        }

        protected virtual EventPreviewViewModel GetPreviewViewModel(EventBase @event, ActivityLinks links)
        {
            var owner = _intranetMemberService.Get(@event);
            return new EventPreviewViewModel
            {
                Id = @event.Id,
                Title = @event.Title,
                Dates = @event.StartDate.GetEventDateTimeString(@event.EndDate).ToListOfOne(),
                Owner = owner.Map<MemberViewModel>(),
                ActivityType = @event.Type,
                Links = links
            };
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

            model.Links = links;
            model.CanHide = _eventsService.CanHide(@event);
			model.PinAllowed = IsPinAllowed();
            return model;
        }

        protected virtual void FillMediaSettingsData(MediaSettings settings)
        {
            ViewData["AllowedMediaExtensions"] = settings.AllowedMediaExtensions;
        }

        protected virtual EventViewModel GetViewModel(EventBase @event, ActivityFeedOptions options)
        {
            var model = @event.Map<EventViewModel>();

            model.CanEdit = _eventsService.CanEdit(@event);
            model.CanSubscribe = _eventsService.CanSubscribe(@event.Id);
            model.Links = options.Links;
            model.IsReadOnly = options.IsReadOnly;

            model.HeaderInfo = @event.Map<IntranetActivityDetailsHeaderViewModel>();
            model.HeaderInfo.Owner = _intranetMemberService.Get(@event).Map<MemberViewModel>();
            model.HeaderInfo.Links = options.Links;

            return model;
        }

        protected virtual EventItemViewModel GetItemViewModel(EventBase @event, IActivityLinks links)
        {
            var model = @event.Map<EventItemViewModel>();

            model.MediaIds = @event.MediaIds;
            model.CanSubscribe = _eventsService.CanSubscribe(@event.Id);
            model.LightboxGalleryPreviewInfo = GetGalleryPreviewInfo(@event);
            model.Links = links;

            model.HeaderInfo = @event.Map<IntranetActivityItemHeaderViewModel>();
            model.HeaderInfo.Owner = _intranetMemberService.Get(@event).Map<MemberViewModel>();
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
            @event.PublishDate = createModel.PublishDate.ToUniversalTime();
            @event.EndDate = createModel.EndDate.ToUniversalTime();
            @event.EndPinDate = createModel.EndPinDate?.ToUniversalTime();
            @event.CreatorId = _intranetMemberService.GetCurrentMemberId();

            if (!IsPinAllowed())
            {
	            @event.EndPinDate = null;
	            @event.IsPinned = false;

            }

            return @event;
        }

        protected virtual EventBase MapToEvent(EventEditModel editModel)
        {
            var @event = MapEditModel(editModel);

            @event.MediaIds = @event.MediaIds.Concat(_mediaHelper.CreateMedia(editModel));
            @event.StartDate = editModel.StartDate.ToUniversalTime().WithCorrectedDaylightSavingTime(editModel.StartDate);
            @event.PublishDate = editModel.PublishDate.ToUniversalTime().WithCorrectedDaylightSavingTime(editModel.PublishDate);
            @event.EndDate = editModel.EndDate.ToUniversalTime().WithCorrectedDaylightSavingTime(editModel.EndDate);
            @event.EndPinDate = editModel.EndPinDate?.ToUniversalTime().WithCorrectedDaylightSavingTime(editModel.EndPinDate.Value);

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