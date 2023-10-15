using AutoMapper;
using Daleya.API.Models;
using Daleya.API.Models.Dto;

namespace Daleya.API
{
    public class MappingConfig : Profile
    {
        //public static MapperConfiguration RegisterMaps()
        //{
        //    var mappingConfig = new MapperConfiguration(config =>
        //    {
        //        config.CreateMap<Category, CategoryDto>().ReverseMap();
        //        config.CreateMap<CategoryDto, Category>().ReverseMap();
        //    });
        //    return mappingConfig;
        //}
        public MappingConfig()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
