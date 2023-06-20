using AutoMapper;
using Core.CommonModels.Enums;
using Posts.Api.ViewModels;

namespace Posts.Api.Dtos
{
    public class GameAutomapper : Profile
    {
        public GameAutomapper()
        {
            CreateMap<Post, CreatePostViewModel>().MapOnlyIfChanged().ReverseMap();
            CreateMap<Post, UpdatePostViewModel>().MapOnlyIfChanged().ReverseMap();
        }
    }
    public static class AutoMapperExpression
    {
        public static IMappingExpression<TSource, TDestination> MapOnlyIfChanged<TSource, TDestination>(this IMappingExpression<TSource, TDestination> map)
        {
            map.ForAllMembers(source =>
            {
                source.Condition((sourceObject, destObject, sourceProperty, destProperty) =>
                {
                    if (sourceProperty == null)
                        return !(destProperty == null);
                    return !sourceProperty.Equals(destProperty);
                });
            });
            return map;
        }
    }
}
