﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uintra20.Core.Activity.Entities;
using Uintra20.Core.Activity.Models;

namespace Uintra20.Core.Activity
{
    public interface IIntranetActivityService<out TActivity> : IIntranetActivityService where TActivity : IIntranetActivity
    {
        TActivity Get(Guid id);
        IEnumerable<TActivity> GetManyActual();
        IEnumerable<TActivity> GetAll(bool includeHidden = false);
        bool IsActual(IIntranetActivity activity);
        bool IsPinActual(IIntranetActivity activity);
        Guid Create(IIntranetActivity activity);
        void Save(IIntranetActivity activity);
        bool CanEdit(IIntranetActivity activity);
        bool CanDelete(IIntranetActivity activity);
        //Task<TActivity> GetAsync(Guid id);
        //Task<IEnumerable<TActivity>> GetManyActualAsync();
        //Task<IEnumerable<TActivity>> GetAllAsync(bool includeHidden = false);
        Task<Guid> CreateAsync(IIntranetActivity activity);
        Task SaveAsync(IIntranetActivity activity);
        IntranetActivityPreviewModelBase GetPreviewModel(Guid activityId, bool showGroupTitle);
    }


    public interface IIntranetActivityService : ITypedService
    {
        void Delete(Guid id);
        bool CanEdit(Guid id);
        bool CanDelete(Guid id);
        Task DeleteAsync(Guid id);
        Task<bool> CanEditAsync(Guid id);
        Task<bool> CanDeleteAsync(Guid id);
    }
}
