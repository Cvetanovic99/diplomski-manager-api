using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.Application.Dtos;

namespace Manager.Application.Interfaces
{
    public interface IProjectService
    {
        Task<PaginationResponse<ProjectDto>> GetProjectsAsync(ProjectPaginationParameters paginationParameters);
        Task<IList<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto> GetProjectByIdAsync(int projectId);
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto updateProjectDto);
        Task DeleteProjectAsync(int projectId);
        Task<PaginationResponse<ProjectTaskWithProductDto>> GetUserProjectTasksAsync(int projectId, TaskPaginationParameters paginationParameters);
    }
}