﻿using System;
using System.Collections.Generic;
using System.Linq;
using Uintra.Core.Caching;
using Uintra.Core.Extensions;
using Uintra.Core.Permissions;
using Uintra.Core.Permissions.Interfaces;
using Uintra.Core.Permissions.Models;
using Uintra.Core.Persistence;
using Uintra.Core.User;
using Uintra.Groups.Sql;
using Umbraco.Core.Models;
using static LanguageExt.Prelude;

namespace Uintra.Groups
{
    public class GroupService : IGroupService
    {
        private const string GroupCacheKey = "Groups";
        private readonly ISqlRepository<Group> _groupRepository;
        private readonly ICacheService _memoryCacheService;
        private readonly IPermissionsService _permissionsService;
        private readonly IIntranetMemberService<IIntranetMember> _intranetMemberService;
        private readonly ISqlRepository<GroupMember> _groupMemberRepository;

        protected const string GroupsCreatePage = "groupsCreatePage";
        protected virtual Enum PermissionResourceType => PermissionResourceTypeEnum.Groups;

        public GroupService(
            ISqlRepository<Group> groupRepository,
            ICacheService memoryCacheService,
            IPermissionsService permissionsService,
            IIntranetMemberService<IIntranetMember> intranetMemberService,
            ISqlRepository<GroupMember> groupMemberRepository)
        {
            _groupRepository = groupRepository;
            _memoryCacheService = memoryCacheService;
            _permissionsService = permissionsService;
            _intranetMemberService = intranetMemberService;
            _groupMemberRepository = groupMemberRepository;
        }

        public Guid Create(GroupModel model)
        {
            var date = DateTime.UtcNow;
            var group = model.Map<Group>();
            group.CreatedDate = date;
            group.UpdatedDate = date;
            group.Id = Guid.NewGuid();

            _groupRepository.Add(group);
            UpdateCache();

            return group.Id;
        }

        public void Edit(GroupModel model)
        {
            var date = DateTime.UtcNow;
            var group = model.Map<Group>();
            group.UpdatedDate = date;
            _groupRepository.Update(group);
            UpdateCache();
        }

        public GroupModel Get(Guid id) =>
            GetAll().SingleOrDefault(g => g.Id == id);

        public IEnumerable<GroupModel> GetAll()
        {
            var groups = _memoryCacheService.GetOrSet(GroupCacheKey, () => _groupRepository.GetAll().ToList(), GetCacheExpiration());

            return groups.Map<IEnumerable<GroupModel>>();
        }

        public IEnumerable<GroupModel> GetAllNotHidden() =>
            GetAll().Where(g => !g.IsHidden);

        public IEnumerable<GroupModel> GetMany(IEnumerable<Guid> groupIds) =>
            GetAllNotHidden().Join(groupIds, g => g.Id, identity, (g, _) => g);

        public bool CanHide(Guid id) =>
            CanHide(Get(id));

        public bool CanHide(GroupModel group) =>
            CanPerform(group);

        public bool CanEdit(Guid id) =>
            CanEdit(Get(id));

        public bool CanEdit(GroupModel group) =>
            CanPerform(group);

        public bool CanPerform(GroupModel group)
        {
            var currentMember = _intranetMemberService.GetCurrentMember();

            var isOwner = group.CreatorId == currentMember.Id;

            var groupMember = _groupMemberRepository.Find(m => m.GroupId == group.Id && m.MemberId == currentMember.Id);

            if (groupMember == null) return false;
            
            var act = isOwner || groupMember.IsAdmin;

            return act;
        }

        public bool ValidatePermission(IPublishedContent content)
        {
            if (content.DocumentTypeAlias == GroupsCreatePage)
            {
                return _permissionsService.Check(PermissionSettingIdentity.Of(PermissionActionEnum.Create, PermissionResourceType));
            }

            return true;
        }

        public void Hide(Guid id)
        {
            var group = Get(id);

            group.IsHidden = true;

            Edit(group);
        }

        public void Unhide(Guid id)
        {
            var group = Get(id);

            group.IsHidden = false;

            Edit(group);
        }
        public bool IsMemberCreator(Guid memberId, Guid groupId) =>
            _groupRepository.Get(groupId)?.CreatorId.CompareTo(memberId) == 0;

        public bool IsActivityFromActiveGroup(IGroupActivity groupActivity) =>
            groupActivity.GroupId.HasValue && !Get(groupActivity.GroupId.Value).IsHidden;

        private static DateTimeOffset GetCacheExpiration() =>
            DateTimeOffset.Now.AddDays(1);

        private void UpdateCache() =>
            _memoryCacheService.Set(GroupCacheKey, _groupRepository.GetAll().ToList(), GetCacheExpiration());
    }
}