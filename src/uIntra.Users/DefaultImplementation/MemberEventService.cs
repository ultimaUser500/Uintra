﻿using Uintra.Core.Permissions;
using Uintra.Core.UmbracoEventServices;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Uintra.Users
{
    public class MemberEventService : 
        IUmbracoMemberDeletingEventService, 
        IUmbracoMemberCreatedEventService,
        IUmbracoMemberAssignedRolesEventService,
        IUmbracoMemberRemovedRolesEventService
    {
        private readonly ICacheableIntranetMemberService _cacheableIntranetMemberService;
        private readonly IMemberService _memberService;
        private readonly IIntranetMemberGroupService _intranetMemberGroupService;

        public MemberEventService(
            ICacheableIntranetMemberService cacheableIntranetMemberService,
            IMemberService memberService,
            IIntranetMemberGroupService intranetMemberGroupService
            )
        {
            _cacheableIntranetMemberService = cacheableIntranetMemberService;
            _memberService = memberService;
            _intranetMemberGroupService = intranetMemberGroupService;
        }

        public void ProcessMemberCreated(IMemberService sender, NewEventArgs<IMember> args)
        {
            var member = args.Entity;
            _intranetMemberGroupService.AssignDefaultMemberGroup(member.Id);
        }

        public void ProcessMemberDeleting(IMemberService sender, DeleteEventArgs<IMember> args)
        {
            foreach (var member in args.DeletedEntities)
            {
                member.IsLockedOut = true;
                _memberService.Save(member);
                _cacheableIntranetMemberService.UpdateMemberCache(member.Key);
            }

            if (args.CanCancel)
            {
                args.Cancel = true;
            }
        }

        public void ProcessMemberAssignedRoles(IMemberService sender, RolesEventArgs e)
        {
            foreach (var memberId in e.MemberIds)
            {
                _cacheableIntranetMemberService.UpdateMemberCache(memberId);
            }
        }

        public void ProcessMemberRemovedRoles(IMemberService sender, RolesEventArgs e)
        {
            foreach (var memberId in e.MemberIds)
            {
                _cacheableIntranetMemberService.UpdateMemberCache(memberId);
            }
        }
    }
}
