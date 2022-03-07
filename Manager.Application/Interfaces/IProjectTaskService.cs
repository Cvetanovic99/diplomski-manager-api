using Manager.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Interfaces
{
    public interface IProjectTaskService
    { 
        Task<ProjectTaskDto> GetProjectTaskByIdAsync(int projectTaskId);
        Task<ProjectTaskDto> CreateProjectTaskAsync(CreateProjectTaskDto projectTaskDto);
        Task<ProjectTaskDto> UpdateProjectTaskAsync(UpdateProjectTaskDto updateProjectTaskDto);
        Task DeleteProjectTaskAsync(int projectTaskId);
        Task<PaginationResponse<ProjectTaskDto>>GetUsedProductAsync(int productId, PaginationParameters paginationParameters);
        Task<IList<ProjectTaskDto>> CreateProjectTasksAsync(IList<CreateProjectTaskDto> projectTaskDtos);
    }
}
