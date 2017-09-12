﻿using System;
using System.Collections.Generic;
using uIntra.Core.User;
using uIntra.Groups.Sql;

namespace uIntra.Groups
{
    public interface IGroupService
    {
        void Create(Group group);

        void Edit(Group group);

        Group Get(Guid id);
        
        IEnumerable<Group> GetAllNotHidden();              

        IEnumerable<Group> GetMany(IEnumerable<Guid> groupIds);

        IEnumerable<Group> GetAllHided();

        IEnumerable<Group> GetAll();

        bool CanCreate(IHaveCreator activity, IIntranetUser user);

        bool CanEdit(Guid groupId, IIntranetUser user);

        bool CanEdit(Group group, IIntranetUser user);

        void Hide(Guid id);
        void UpdateGroupUpdateDate(Guid id);
        void Unhide(Guid id);
        void FillGroupActivityData(IGroupActivity activity, bool isGroupPage);

        void AddGroupActivityRelation(Guid groupId, Guid activityId);
        void RemoveGroupActivityRelation(Guid groupId, Guid activityId);
        Guid? GetGroupIdFromQuery(string query);
    }
}
