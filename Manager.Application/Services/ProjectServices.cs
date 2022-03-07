using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Manager.Application.Dtos;
using Manager.Application.Exceptions;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Application.Specifications;
using Manager.Core.Entities;

namespace Manager.Application.Services
{
    public class ProjectServices : IProjectService
    {
        private readonly IAsyncRepository<Project> _projectRepository;
        private readonly IAsyncRepository<User> _userRepository;
        private readonly IAsyncRepository<Client> _clientRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<ProjectTask> _projectTaskRepository;
        private readonly IAsyncRepository<Product> _productRepository;

        private readonly IMapper _mapper;

        public ProjectServices(
            IAsyncRepository<Project> projectRepository,
            IAsyncRepository<User> userRepository,
            IAsyncRepository<Client> clientRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<ProjectTask> projectTaskRepository,
            IAsyncRepository<Product> productRepository,
            IMapper mapper)
        {
            this._projectRepository = projectRepository;
            this._userRepository = userRepository;
            this._clientRepository = clientRepository;
            this._warehouseRepository = warehouseRepository;
            this._projectTaskRepository = projectTaskRepository;
            this._productRepository = productRepository;
            this._mapper = mapper;
        }
        
        public async Task<PaginationResponse<ProjectDto>> GetProjectsAsync(ProjectPaginationParameters paginationParameters)
        {
            Expression<Func<Project, bool>> criteria = p => (p.Title.Contains(paginationParameters.Keyword) || p.Client.ClientId.Contains(paginationParameters.Keyword) || p.Client.Name.Contains(paginationParameters.Keyword)) &&
                                                            (string.IsNullOrEmpty(paginationParameters.State) || p.State.Equals(paginationParameters.State));
            
            var projects = await _projectRepository.GetBySpecAsync(
                new ProjectSpecification(
                    criteria: criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    orderBy: paginationParameters.OrderBy,
                    direction: paginationParameters.Direction));

            var count = await _projectRepository.GetCountBySpecAsync(
                new ProjectSpecification(criteria));

            return new PaginationResponse<ProjectDto>()
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<ProjectDto>>(projects),
                Total = count
            };
        }

        public async Task<IList<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAsync();

            return _mapper.Map<IList<ProjectDto>>(projects);
        }

        public async Task<ProjectDto> GetProjectByIdAsync(int projectId)
        {
            var project = await _projectRepository.GetSingleBySpecAsync(
                new ProjectByIdSpecification(projectId));

            if (project is null)
            {
                throw new ApiException("Projekat nije pronadjen", 404);
            }

            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto)  
        {
            var client = await _clientRepository.GetByIdAsync(createProjectDto.ClientId);
            if (client is null) throw new ApiException("Klijent nije pronadjen", 404);

            var warehouse = await _warehouseRepository.GetByIdAsync(createProjectDto.WarehouseId);
            if (warehouse is null) throw new ApiException("Magacin nije pronadjen", 404);


            var project = _mapper.Map<Project>(createProjectDto);
            project.Client = client;
            project.Warehouse = warehouse;

            if (createProjectDto.State == null)
                project.State = "U procesu"; 

            await _projectRepository.AddAsync(project);
            
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto updateProjectDto)
        {
            var project = await _projectRepository.GetSingleBySpecAsync(new ProjectByIdSpecification(updateProjectDto.Id));
            if (project is null) throw new ApiException("Projekat nije pronadjen", 404);

            if (project.Client.Id != updateProjectDto.ClientId)
            {
                var client = await _clientRepository.GetByIdAsync(updateProjectDto.ClientId);
                if (client is null) throw new ApiException("Klijent nije pronadjen", 404);
                project.Client = client;
            }
            
            if (project.Warehouse.Id != updateProjectDto.WarehouseId)
            {
                var warehouse = await _warehouseRepository.GetByIdAsync(updateProjectDto.WarehouseId);
                if (warehouse is null) throw new ApiException("Magacin nije pronadjen", 404);
                project.Warehouse = warehouse;
            }

            _mapper.Map(updateProjectDto, project);

            await _projectRepository.UpdateAsync(project);
            
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task DeleteProjectAsync(int projectId)
        {
            var project = await _projectRepository.GetSingleBySpecAsync(new ProjectByIdSpecification(projectId));
            if (project is null) throw new ApiException("Projekat nije pronadjen", 404);

            await _projectRepository.DeleteAsync(project);
        }

        public async Task<PaginationResponse<ProjectTaskWithProductDto>> GetUserProjectTasksAsync(int projectId, TaskPaginationParameters paginationParameters)
        {

            Expression<Func<ProjectTask, bool>> criteria = pt => pt.Project.Id == projectId &&
                                                                 pt.Description.Contains(paginationParameters.Keyword) &&
                                                                 (
                                                                     paginationParameters.UserId == null || 
                                                                     (pt.Employed1.Id == paginationParameters.UserId || pt.Employed2.Id == paginationParameters.UserId)
                                                                 );

            var tasks = await _projectTaskRepository.GetBySpecAsync(
                new ProjectTaskSpecification(
                    criteria: criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    paginationParameters.OrderBy,
                    paginationParameters.Direction));

            var count = await _projectTaskRepository.GetCountBySpecAsync(
                new ProjectTaskSpecification(criteria));
            
            IList<ProjectTaskWithProductDto>  items = new List<ProjectTaskWithProductDto>();
            foreach (var task in tasks)
            {
                var projectTaskWithProduct = _mapper.Map<ProjectTaskWithProductDto>(task);
                projectTaskWithProduct.Product = task.ProductState is null ? null : _mapper.Map<ProductDto>(await _productRepository.GetByIdAsync(task.ProductState.ProductId));
                items.Add(projectTaskWithProduct);
            }

            return new PaginationResponse<ProjectTaskWithProductDto>
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = items,
                Total = count
            };
        }
    }
}