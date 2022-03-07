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
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ProjectTask> _projectTaskRepository;
        private readonly IAsyncRepository<Project> _projectRepository;
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly IAsyncRepository<ProductState> _productStateRepository;
        private readonly IAsyncRepository<User> _userRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;


        public ProjectTaskService(
            IAsyncRepository<ProjectTask> projectTaskRepository,
            IAsyncRepository<Project> projectRepository,
            IAsyncRepository<Product> productRepository,
            IAsyncRepository<ProductState> productStateRepository,
            IAsyncRepository<User> userRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IMapper mapper)
        {
            this._projectTaskRepository = projectTaskRepository;
            this._projectRepository = projectRepository;
            this._productRepository = productRepository;
            this._productStateRepository = productStateRepository;
            _userRepository = userRepository;
            _warehouseRepository = warehouseRepository;
            this._mapper = mapper;

        }

        public async Task<ProjectTaskDto> CreateProjectTaskAsync(CreateProjectTaskDto projectTaskDto)
        {
            ProjectTask projectTask = null;

            var project = await _projectRepository.GetSingleBySpecAsync(new ProjectByIdSpecification(projectTaskDto.ProjectId));
            if (project is null) throw new ApiException("Projekat nije pronadjen", 404);

            var employed1 = await _userRepository.GetByIdAsync(projectTaskDto.Employed1Id);
            if (employed1 is null)
                throw new ApiException("Zaposleni nije pronadjen", 404);

            User employed2 = null;
            if (projectTaskDto.Employed2Id is not null)
            {
                employed2 = await _userRepository.GetByIdAsync(projectTaskDto.Employed2Id.Value);
                if (employed2 is null)
                    throw new ApiException("Zaposleni nije pronadjen", 404);
            }

            if (projectTaskDto.ProductStateId != null)
            {
                var productState = await _productStateRepository.GetSingleBySpecAsync(new ProductStateByIdSpecification(projectTaskDto.ProductStateId.Value));
                if (productState is null)
                    throw new ApiException("Stanje proizvoda nije pronadjeno", 404);

                if (productState.Quantity < projectTaskDto.QuantityUsed)
                {
                    throw new ApiException("Nema dovoljno proizvoda.", 400);
                }
                else
                {
                    productState.Quantity = productState.Quantity - projectTaskDto.QuantityUsed;
                    await _productStateRepository.UpdateAsync(productState);
                }

                projectTask = _mapper.Map<ProjectTask>(projectTaskDto);
                projectTask.ProductState = productState;
                projectTask.Project = project;
            }
            else
            {
                projectTask = _mapper.Map<ProjectTask>(projectTaskDto);
                projectTask.Project = project;
            }

            projectTask.Employed1 = employed1;
            projectTask.Employed2 = employed2;
            
            await _projectTaskRepository.AddAsync(projectTask);
            
            return _mapper.Map<ProjectTaskDto>(projectTask);
        }

        public async Task DeleteProjectTaskAsync(int projectTaskId)
        {
            var projectTask =
                await _projectTaskRepository.GetSingleBySpecAsync(new ProjectTaskByIdspecification(projectTaskId));
            if (projectTask is null)
                throw new ApiException("Task nije pronadjen", 404);

            if (projectTask.ProductState is not null)
            {
                var oldProductStateFromDatbase = await _productStateRepository.GetByIdAsync(projectTask.ProductState.Id);
                if (oldProductStateFromDatbase is null)
                    throw new ApiException("Stanje proizvoda nije pronadjeno.", 404);

                oldProductStateFromDatbase.Quantity = oldProductStateFromDatbase.Quantity + projectTask.QuantityUsed;
                await _productStateRepository.UpdateAsync(oldProductStateFromDatbase);
            }

            await _projectTaskRepository.DeleteAsync(projectTask);

        }

        public async Task<ProjectTaskDto> GetProjectTaskByIdAsync(int projectTaskId)
        {

            var projectTask =
                await _projectTaskRepository.GetSingleBySpecAsync(new ProjectTaskByIdspecification(projectTaskId));

            if (projectTask is null)
            {
                throw new ApiException("Task nije pronadjen", 404);
            }

            return _mapper.Map<ProjectTaskDto>(projectTask);
        }

        public async Task<ProjectTaskDto> UpdateProjectTaskAsync(UpdateProjectTaskDto updateProjectTaskDto)
        {
            //TODO: Ako se nekad ubaci funkcionalnost da se menja i projekat morace i to da se ispita. Za sada samo proizvod

            var projectTask =
                await _projectTaskRepository.GetSingleBySpecAsync(
                    new ProjectTaskByIdspecification(updateProjectTaskDto.Id));
            if (projectTask is null)
                throw new ApiException("Task nije pronadjen", 404);

            var project =
                await _projectRepository.GetSingleBySpecAsync(new ProjectByIdSpecification(projectTask.Project.Id));
            if (project is null)
                throw new ApiException("Projekat za task nije pronadjen.", 404);

            if (projectTask.Employed1.Id != updateProjectTaskDto.Employed1Id)
            {
                var employed1 = await _userRepository.GetByIdAsync(updateProjectTaskDto.Employed1Id);
                if (employed1 is null)
                    throw new ApiException("Zaposleni nije pronadjen", 404);
                projectTask.Employed1 = employed1;
            }

            projectTask.Employed2 = null;
            if (updateProjectTaskDto.Employed2Id is not null)
            {
                var employed2 = await _userRepository.GetByIdAsync(updateProjectTaskDto.Employed2Id.Value);
                if (employed2 is null)
                    throw new ApiException("Zaposleni nije pronadjen", 404);
                projectTask.Employed2 = employed2;
            }

            if (projectTask.ProductState is not null) //Ako proizvod nije null
            {
                if (updateProjectTaskDto.ProductStateId is not null) //Ako u update ima proizvod
                {
                    if (updateProjectTaskDto.ProductStateId !=
                        projectTask.ProductState.Id) //Ako se razlikuje vec postojeci proizvod i onaj koji saljemo 
                    {
                        var newProductStateFromDatbase =
                            await _productStateRepository.GetSingleBySpecAsync(
                                new ProductStateByIdSpecification(updateProjectTaskDto.ProductStateId.Value));
                        if (newProductStateFromDatbase is null)
                            throw new ApiException("Novo stanje proizvoda nije pronadjeno", 404);

                        if (newProductStateFromDatbase.Quantity < updateProjectTaskDto.QuantityUsed)
                        {
                            throw new ApiException("Nema dovoljno proizvoda.", 400);
                        }
                        else
                        {
                            newProductStateFromDatbase.Quantity = newProductStateFromDatbase.Quantity -
                                                                  updateProjectTaskDto.QuantityUsed;
                            await _productStateRepository.UpdateAsync(newProductStateFromDatbase);
                        }

                        var oldProductStateFromDatbase =
                            await _productStateRepository.GetByIdAsync(projectTask.ProductState.Id);
                        if (oldProductStateFromDatbase != null)
                        {
                            oldProductStateFromDatbase.Quantity =
                                oldProductStateFromDatbase.Quantity + projectTask.QuantityUsed;
                            await _productStateRepository.UpdateAsync(oldProductStateFromDatbase);
                        }

                        _mapper.Map(updateProjectTaskDto, projectTask);
                        projectTask.ProductState = newProductStateFromDatbase;
                        projectTask.Project = project;
                    }
                    else
                    {
                        var oldProductStateFromDatabase =
                            await _productStateRepository.GetByIdAsync(projectTask.ProductState.Id);
                        if (oldProductStateFromDatabase is null)
                            throw new ApiException("Stanje proizvoda nije pronadjeno.", 404);

                        oldProductStateFromDatabase.Quantity += projectTask.QuantityUsed;

                        if (oldProductStateFromDatabase.Quantity < updateProjectTaskDto.QuantityUsed)
                            throw new ApiException("Nema dovoljno proizvoda.", 400);

                        oldProductStateFromDatabase.Quantity -= updateProjectTaskDto.QuantityUsed;
                        await _productStateRepository.UpdateAsync(oldProductStateFromDatabase);

                        _mapper.Map(updateProjectTaskDto, projectTask);
                        projectTask.ProductState = oldProductStateFromDatabase;
                        projectTask.Project = project;
                    }
                }
                else
                {
                    var oldProductStateFromDatbase =
                        await _productStateRepository.GetByIdAsync(projectTask.ProductState.Id);
                    if (oldProductStateFromDatbase is null)
                        throw new ApiException("Stanje proizvoda nije pronadjeno.", 404);

                    oldProductStateFromDatbase.Quantity =
                        oldProductStateFromDatbase.Quantity + projectTask.QuantityUsed;
                    await _productStateRepository.UpdateAsync(oldProductStateFromDatbase);

                    projectTask.ProductState = null;
                    _mapper.Map(updateProjectTaskDto, projectTask);
                }
            }
            else
            {
                if (updateProjectTaskDto.ProductStateId is not null)
                {
                    var newProductStateFromDatabase =
                        await _productStateRepository.GetByIdAsync(updateProjectTaskDto.ProductStateId.Value);
                    if (newProductStateFromDatabase is null)
                        throw new ApiException("Novo stanje proizvoda nije pronadjeno", 404);

                    if (newProductStateFromDatabase.Quantity < updateProjectTaskDto.QuantityUsed)
                    {
                        throw new ApiException("Nema dovoljno proizvoda.", 400);
                    }
                    else
                    {
                        newProductStateFromDatabase.Quantity =
                            newProductStateFromDatabase.Quantity - updateProjectTaskDto.QuantityUsed;
                        await _productStateRepository.UpdateAsync(newProductStateFromDatabase);
                    }

                    _mapper.Map(updateProjectTaskDto, projectTask);
                    projectTask.ProductState = newProductStateFromDatabase;
                }
                else
                {
                    _mapper.Map(updateProjectTaskDto, projectTask);
                }
            }

            await _projectTaskRepository.UpdateAsync(projectTask);

            return _mapper.Map<ProjectTaskDto>(projectTask);
        }

        public async Task<PaginationResponse<ProjectTaskDto>> GetUsedProductAsync(int productId,
            PaginationParameters paginationParameters)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null) throw new ApiException("Proizvod ne postoji", 404);

            Expression<Func<ProjectTask, bool>> criteria = pt =>
                pt.ProductState.Product == product && pt.ProductState.SN.Contains(paginationParameters.Keyword);

            var projectTasks = await _projectTaskRepository.GetBySpecAsync(new ProjectTaskWithProjectSpecification(
                criteria: criteria,
                start: paginationParameters.PageIndex * paginationParameters.PageSize,
                take: paginationParameters.PageSize,
                direction: paginationParameters.Direction,
                orderBy: paginationParameters.OrderBy));

            var count = await _projectTaskRepository.GetCountBySpecAsync(
                new ProjectTaskSpecification(criteria));

            return new PaginationResponse<ProjectTaskDto>()
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<ProjectTaskDto>>(projectTasks),
                Total = count
            };

        }

        public async Task<IList<ProjectTaskDto>> CreateProjectTasksAsync(IList<CreateProjectTaskDto> projectTaskDtos)
        {
            List<ProjectTaskDto> projectTasksToReturn = new List<ProjectTaskDto>();
            List<ProjectTask> projectTasksToAdd = new List<ProjectTask>();
            
            foreach (var projectTaskDto in projectTaskDtos)
            {
                ProjectTask projectTask = null;

                var project = await _projectRepository.GetSingleBySpecAsync(new ProjectByIdSpecification(projectTaskDto.ProjectId));
                if (project is null) throw new ApiException("Projekat nije pronadjen", 404);

                var employed1 = await _userRepository.GetByIdAsync(projectTaskDto.Employed1Id);
                if (employed1 is null)
                    throw new ApiException("Zaposleni nije pronadjen", 404);

                User employed2 = null;
                if (projectTaskDto.Employed2Id is not null)
                {
                    employed2 = await _userRepository.GetByIdAsync(projectTaskDto.Employed2Id.Value);
                    if (employed2 is null)
                        throw new ApiException("Zaposleni nije pronadjen", 404);
                }

                if (projectTaskDto.ProductStateId != null)
                {
                    var productState = await _productStateRepository.GetSingleBySpecAsync(new ProductStateByIdSpecification(projectTaskDto.ProductStateId.Value));
                    if (productState is null)
                        throw new ApiException("Stanje proizvoda nije pronadjeno", 404);

                    if (productState.Quantity < projectTaskDto.QuantityUsed)
                    {
                        throw new ApiException("Nema dovoljno proizvoda.", 400);
                    }
                    else
                    {
                        productState.Quantity = productState.Quantity - projectTaskDto.QuantityUsed;
                        await _productStateRepository.UpdateAsync(productState);
                    }

                    projectTask = _mapper.Map<ProjectTask>(projectTaskDto);    
                    projectTask.ProductState = productState;
                    projectTask.Project = project;
                }
                else
                {
                    projectTask = _mapper.Map<ProjectTask>(projectTaskDto);
                    projectTask.Project = project;
                }

                projectTask.Employed1 = employed1;
                projectTask.Employed2 = employed2;
                
                projectTasksToAdd.Add(projectTask);
                //projectTasksToReturn.Add(_mapper.Map<ProjectTaskDto>(projectTask));
            }

            await _projectTaskRepository.AddRangeAsync(projectTasksToAdd);

            return projectTasksToReturn = _mapper.Map<List<ProjectTaskDto>>(projectTasksToAdd);
        }
    }
}
