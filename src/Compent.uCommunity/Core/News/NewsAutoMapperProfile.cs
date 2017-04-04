﻿using AutoMapper;
using Compent.uCommunity.Core.News.Models;
using uCommunity.Core.Activity.Models;
using uCommunity.News;

namespace Compent.uCommunity.Core.News
{
    public class NewsAutoMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Entities.News, NewsViewModel>()
                .IncludeBase<NewsModelBase, NewsViewModel>()
                .IncludeBase<NewsModelBase, NewsViewModelBase>()
                .ForMember(dst => dst.LikesInfo, o => o.MapFrom(el => el))
                .ForMember(dst => dst.CommentsInfo, o => o.MapFrom(el => el));

            Mapper.CreateMap<Entities.News, NewsOverviewItemModel>()
                .IncludeBase<NewsModelBase, NewsOverviewItemModel>()
                .ForMember(dst => dst.LikesInfo, o => o.MapFrom(el => el));

            Mapper.CreateMap<Entities.News, IntranetActivityHeaderModel>()
                 .IncludeBase<NewsModelBase, IntranetActivityHeaderModel>();
        }
    }
}