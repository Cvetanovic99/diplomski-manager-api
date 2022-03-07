using AutoMapper;
using Manager.Application.Dtos;
using Manager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Profiles
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile()
        {
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<WarehouseDto, Warehouse>();
            CreateMap<CreateWarehouseDto, Warehouse>();

            CreateMap<Warehouse, WarehouseProductDto>();
        }
    }
}
