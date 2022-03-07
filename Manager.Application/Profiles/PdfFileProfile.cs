using AutoMapper;
using Manager.Application.Dtos;
using Manager.Core.Entities;

namespace Manager.Application.Profiles
{
    public class PdfFileProfile : Profile
    {
        public PdfFileProfile()
        {
            CreateMap<PdfFile, PdfFileDto>();
        }
    }
}