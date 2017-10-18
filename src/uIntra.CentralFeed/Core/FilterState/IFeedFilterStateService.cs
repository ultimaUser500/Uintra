﻿namespace uIntra.CentralFeed
{
    public interface IFeedFilterStateService
    {
        void SaveFiltersState(FeedFiltersState stateModel);
        T GetFiltersState<T>();
        bool CentralFeedCookieExists();
    }
}