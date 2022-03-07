using AutoMapper;
using Manager.Application.Dtos;
using Manager.Core.Entities;

namespace Manager.Application.Profiles
{
    public class ProjectTaskProfile : Profile
    {
        public ProjectTaskProfile()
        {
            CreateMap<ProjectTask, ProjectTaskDto>();
            CreateMap<ProjectTaskDto, ProjectTask>();
            CreateMap<CreateProjectTaskDto, ProjectTask>();
            CreateMap<UpdateProjectTaskDto, ProjectTask>();
            CreateMap<ProjectTask, UpdateProjectTaskDto>();
            CreateMap<ProjectTask, ProjectTaskWithProductDto>();

        }
    }
}