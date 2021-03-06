﻿using System.Collections.Generic;
using Uintra.Core.Permissions.Models;

namespace Uintra.Core.Permissions
{
    public interface IIntranetMemberGroupService
    {
        IEnumerable<IntranetMemberGroup> GetAll();
        IEnumerable<IntranetMemberGroup> GetForMember(int id);
        int Create(string name);
        bool Save(int id, string name);
        void Delete(int id);
        void AssignDefaultMemberGroup(int memberId);
        void ClearCache();
    }
}