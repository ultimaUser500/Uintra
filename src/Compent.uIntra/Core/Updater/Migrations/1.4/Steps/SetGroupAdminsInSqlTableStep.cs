﻿using System;
using Uintra.Groups;

namespace Compent.Uintra.Core.Updater.Migrations._1._4.Steps
{
    public class SetGroupAdminsInSqlTableStep : IMigrationStep
    {
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;

        public SetGroupAdminsInSqlTableStep(IGroupService groupService, IGroupMemberService groupMemberService)
        {
            _groupService = groupService;
            _groupMemberService = groupMemberService;
        }
        public ExecutionResult Execute()
        {
            var groups = _groupService.GetAll();

            foreach (var group in groups)
            {
                var creator = _groupMemberService.GetGroupMemberByMemberIdAndGroupId(group.CreatorId, group.Id);

                if(creator == null) continue;

                if (creator.IsAdmin) continue;

                creator.IsAdmin = true;
                _groupMemberService.Update(creator);
            }

            return ExecutionResult.Success;
        }

        public void Undo()
        {
        }
    }
}