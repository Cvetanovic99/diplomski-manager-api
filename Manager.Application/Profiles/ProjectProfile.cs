using System.Linq;
using AutoMapper;
using Manager.Application.Dtos;
using Manager.Core.Entities;

namespace Manager.Application.Profiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();
            CreateMap<Project, ProjectDto>()
                .ForMember(p => p.PdfFile,
                 opt => opt.MapFrom(opt => opt.PdfFiles.Count > 0 ? opt.PdfFiles.First() : null));
            CreateMap<Project, ProjectExcelDto>();
        }
    }
}