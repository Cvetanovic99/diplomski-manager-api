using AutoMapper;
using Manager.Application.Dtos;
using Manager.Core.Entities;

namespace Manager.Application.Profiles
{
    public class ProductStateProfile : Profile
    {
        public ProductStateProfile()
        {
            CreateMap<ProductState, ProductStateDto>();
            CreateMap<UpdateProductStateDto, ProductState>();
        }
    }
}