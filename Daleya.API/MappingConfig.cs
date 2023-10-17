using AutoMapper;
using Daleya.API.Models;
using Daleya.API.Models.Dto;

namespace Daleya.API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
