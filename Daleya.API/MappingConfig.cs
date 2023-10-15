using AutoMapper;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Create;

namespace Daleya.API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
        }
    }
}
