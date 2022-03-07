using AutoMapper;
using Manager.Application.Dtos;
using Manager.Core.Entities;
using System.Linq;

namespace Manager.Application.Profiles
{
    public class ProductProfile: Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
            CreateMap<Product, ProductDto>();

            CreateMap<Product, ProductWarehouseDto>();
            // .ForMember(pw => pw.States,
            //     opt => opt.MapFrom(opt => opt.States.First()));
        }
    }
}
