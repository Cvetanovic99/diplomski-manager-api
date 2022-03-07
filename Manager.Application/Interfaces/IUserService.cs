using Manager.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Interfaces
{
    public interface IUserService
    {
        Task<PaginationResponse<UserDto>> GetUsersAsync(PaginationParameters paginationParameters);
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<UserDto> GetUserByIdentityId(string identityId);
        Task CreateUserAsync(CreateUserDto createUserDto);
        Task CreateUserAsync(string identityId);
        Task UpdateUserAsync(UpdateUserDto updateUserDto, string token);
        Task DeleteUserAsync(int userId);
        Task<PaginationResponse<ProjectDto>> GetUserProjectsAsync(int employeeId, ProjectPaginationParameters paginationParameters);
        Task<ExcelDto> GetProjectsWithTasksBetweenDatesExcelAsync(int employeeId, ReportQueryParameters reportQueryParameters);
        Task<IList<ProjectDto>> GetAllUserProjectsAsync(int employeeId);
        Task<UserDto> UpdateUserToolsAsync(UpdateUserToolsDto updateUserToolsDto);
        Task<PaginationResponse<UserDto>> GetUserColleaguesAsync(int userId, PaginationParameters paginationParameters);
        Task SetEmployeePassword(int employeeId, PasswordDto passwordDto);
    }
}
