﻿using System;
using System.Linq;
using System.Web.Mvc;
using uIntra.Events;

namespace Compent.uIntra.Core.Updater.Migrations._0._0._0._1.Steps.AggregateSubsteps
{
    public class EventPublishDateMigration
    {
        public void Execute()
        {
            FixEmptyPublishDate();
        }

        private void FixEmptyPublishDate()
        {
            var eventService = DependencyResolver.Current.GetService<IEventsService<EventBase>>();

            var events = eventService.GetAll(true).Where(@event => @event.PublishDate == default(DateTime)).ToList();
            foreach (var @event in events)
            {
                @event.PublishDate = @event.StartDate;
                eventService.Save(@event);
            }
        }
    }
}