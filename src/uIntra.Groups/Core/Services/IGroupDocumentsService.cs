﻿using System;
using System.Collections.Generic;
using Uintra.Groups.Sql;

namespace Uintra.Groups
{
    public interface IGroupDocumentsService
    {
        IEnumerable<GroupDocument> GetByGroup(Guid groupId);
        Guid Create(GroupDocument document);
        void Create(IEnumerable<GroupDocument> documents);
        GroupDocument Get(Guid documentId);
        void Delete(GroupDocument document);
    }
}