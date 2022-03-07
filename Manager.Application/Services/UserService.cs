using AutoMapper;
using Manager.Application.Dtos;
using Manager.Application.Exceptions;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Application.Specifications;
using Manager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IAsyncRepository<User> _userRepository;
        private readonly IAsyncRepository<Project> _projectRepository;
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly IAsyncRepository<ProductState> _productStateRepository;
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UserService(
            IAsyncRepository<User> userRepository, 
            IAsyncRepository<Project> projectRepository,
            IAsyncRepository<Product> productRepository,
            IAsyncRepository<ProductState> productStateRepository,
            IAuthService authService,
            ITokenService tokenService,
            IMapper mapper)
        {
            this._userRepository = userRepository;
            this._projectRepository = projectRepository;
            this._productRepository = productRepository;
            this._productStateRepository = productStateRepository;
            this._authService = authService;
            this._tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = await _userRepository.GetSingleBySpecAsync(
                new UserByIdentityIdSpecification(createUserDto.IdentityId));

            if (user is not null)
            {
                throw new ApiException("Korisnik vec postoji.", 400);
            }
            
            user = _mapper.Map<User>(createUserDto);

            await _userRepository.AddAsync(user);
        }

        public async Task CreateUserAsync(string identityId)
        {
            var user = await _userRepository.GetSingleBySpecAsync(
                new UserByIdentityIdSpecification(identityId));

            if (user is not null)
            {
                throw new ApiException("Korisnik vec postoji.", 400);
            }

            user = new User
            {
                IdentityId = identityId,
            };

            await _userRepository.AddAsync(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                throw new ApiException("Korisnik nije pronadjen.", 404);
            }

            await _userRepository.DeleteAsync(user);
        }

        public async Task UpdateUserAsync(UpdateUserDto updateUserDto, string token)
        {
            // var (_, IdentityId) = _tokenService.GetUserClaimsFromToken(token);
            
            var user = await _userRepository.GetByIdAsync(updateUserDto.Id);
            if (user is null)
            {
                throw new ApiException("Korisnik nije pronadjen.", 404);
            }
            
            await _authService.ChangeEmailAsync(user.IdentityId, new ChangeEmailDto { Email = updateUserDto.Email });



            _mapper.Map(updateUserDto, user);
            await _userRepository.UpdateAsync(user);
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                throw new ApiException("Korisnik nije pronadjen.", 404);
            }

            var email = await _authService.FindEmailByIdAsync(user.IdentityId);
            
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Email = email;

            return userDto;
        }

        public async Task<UserDto> GetUserByIdentityId(string identityId)
        {
            var user = await _userRepository.GetSingleBySpecAsync(
              new UserByIdentityIdSpecification(identityId));

            if (user is null)
            {
                throw new ApiException("Korisnik nije pronadjen.", 404);
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<PaginationResponse<UserDto>> GetUsersAsync(PaginationParameters paginationParameters)
        {
            var adminIdentityId = await _authService.GetAdminId();
            var admin = await _userRepository.GetSingleBySpecAsync(new UserByIdentityIdSpecification(adminIdentityId));
            
            Expression<Func<User, bool>> criteria = u => u.Id != admin.Id && (u.Name.Contains(paginationParameters.Keyword) || u.Surname.Contains(paginationParameters.Keyword));

            var users = await _userRepository.GetBySpecAsync(
                new UserSpecification(
                    criteria: criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    paginationParameters.OrderBy));

            var count = await _userRepository.GetCountBySpecAsync(
                new UserSpecification(criteria));

            return new PaginationResponse<UserDto>
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<UserDto>>(users),
                Total = count
            };
        }

        public async Task<PaginationResponse<ProjectDto>> GetUserProjectsAsync(int employeeId, ProjectPaginationParameters paginationParameters)
        {
            Expression<Func<Project, bool>> criteria = p => (p.Title.Contains(paginationParameters.Keyword) || p.Client.ClientId.Contains(paginationParameters.Keyword)) && 
                                                            (string.IsNullOrEmpty(paginationParameters.State) || p.State.Equals(paginationParameters.State));

            var projects = await _projectRepository.GetBySpecAsync(
                new ProjectSpecification(
                    criteria: criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    paginationParameters.OrderBy));

            var count = await _projectRepository.GetCountBySpecAsync(
                new ProjectSpecification(criteria));

            return new PaginationResponse<ProjectDto>
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<ProjectDto>>(projects),
                Total = count
            };
        }

        public async Task<ExcelDto> GetProjectsWithTasksBetweenDatesExcelAsync(int employeeId, ReportQueryParameters reportQueryParameters)
        {
            var user = await _userRepository.GetByIdAsync(employeeId);

            if (user is null)
            {
                throw new ApiException("Radnik nije pronadjen", 404);
            }
            
            var projects = await _projectRepository.GetBySpecAsync(
                new ProjectByTaskDateSpecification(
                    user,
                    criteria: project =>  
                                         (reportQueryParameters.ProjectId == null || project.Id == reportQueryParameters.ProjectId) &&
                                         project.Tasks.Any(task => task.CreatedAt >= reportQueryParameters.Start && task.CreatedAt <= reportQueryParameters.End && (task.Employed1 == user || task.Employed2 == user)),
                    reportQueryParameters.Start, reportQueryParameters.End)
                );

            Dictionary<int, string> productNames = new Dictionary<int, string>();
            
            foreach(Project project in projects)
            {
                foreach(ProjectTask task in project.Tasks)
                {
                    task.ProductState = task.ProductStateId.HasValue ? await _productStateRepository.GetSingleBySpecAsync(new ProductStateByIdSpecification(task.ProductStateId.Value)) : null;
                    if (task.ProductState is not null)
                    {
                        if (!productNames.ContainsKey(task.ProductState.Product.Id))
                        {
                            productNames.Add(task.ProductState.Product.Id, task.ProductState.Product.Name);
                        }
                    }
                }
            }

            var mappedProjects = _mapper.Map<IList<ProjectExcelDto>>(projects);
            foreach(ProjectExcelDto project in mappedProjects)
            {
                foreach(ProjectTaskDto task in project.Tasks)
                {
                    if (task.ProductState is not null)
                    {
                        productNames.TryGetValue(task.ProductState.ProductId, out var name);
                        task.ProductState.ProductName = name;
                    }
                }
            }
            
            return new ExcelDto()
            {
                User = _mapper.Map<UserDto>(user),
                Projects = mappedProjects
            };
        }

        public async Task<IList<ProjectDto>> GetAllUserProjectsAsync(int employeeId)
        {
            Expression<Func<Project, bool>> criteria = p => p.Tasks.Any(pt => pt.Employed1.Id == employeeId || pt.Employed2.Id == employeeId);

            var projects = await _projectRepository.GetBySpecAsync(new ProjectSpecification(criteria));

            return _mapper.Map<IList<ProjectDto>>(projects);
        }

        public async Task<UserDto> UpdateUserToolsAsync(UpdateUserToolsDto updateUserToolsDto)
        {
            var user = await _userRepository.GetByIdAsync(updateUserToolsDto.Id);
            if (user is null)
                throw new ApiException("Zaposleni nije pronadjen", 404);

            user.Tools = updateUserToolsDto.Tools;

            await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<PaginationResponse<UserDto>> GetUserColleaguesAsync(int userId, PaginationParameters paginationParameters)
        {
            var adminIdentityId = await _authService.GetAdminId();
            var admin = await _userRepository.GetSingleBySpecAsync(new UserByIdentityIdSpecification(adminIdentityId));
            
            Expression<Func<User, bool>> criteria = u => u.Id != userId && u.Id != admin.Id && (u.Name.Contains(paginationParameters.Keyword) || u.Surname.Contains(paginationParameters.Keyword));

            var users = await _userRepository.GetBySpecAsync(
                new UserSpecification(
                    criteria: criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    paginationParameters.OrderBy));

            var count = await _userRepository.GetCountBySpecAsync(
                new UserSpecification(criteria));

            return new PaginationResponse<UserDto>
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<UserDto>>(users),
                Total = count
            };
        }

        public async Task SetEmployeePassword(int employeeId, PasswordDto passwordDto)
        {
            var user = await _userRepository.GetByIdAsync(employeeId);

            if (user is null)
            {
                throw new ApiException("Radnik nije pronadjen", 404);
            }
            
            await _authService.SetPasswordAsync(user.IdentityId, passwordDto.Password);
        }
    }
}
