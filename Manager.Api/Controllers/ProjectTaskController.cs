using Manager.Application.Dtos;
using Manager.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manager.Application.Interfaces.ThirdPartyContracts;

namespace Manager.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class ProjectTaskController : Controller
    {
        private readonly IProjectTaskService _projectTaskService;

        public ProjectTaskController(IProjectTaskService projectTaskService)
        {
            this._projectTaskService = projectTaskService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProjectTask(CreateProjectTaskDto createProjectTaskDto)
        {
            var projectTask = await _projectTaskService.CreateProjectTaskAsync(createProjectTaskDto);

            return Created("", projectTask);
        }
        
        [HttpPost("range")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProjectTasks(IList<CreateProjectTaskDto> createProjectTaskDtos)
        {
            var projectTasks = await _projectTaskService.CreateProjectTasksAsync(createProjectTaskDtos);

            return Created("", projectTasks);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTask(int id)
        {
            var projectTask = await _projectTaskService.GetProjectTaskByIdAsync(id);
            

            return Ok(projectTask);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWarehouse(int id, UpdateProjectTaskDto updateProjectTaskDto)
        {
            updateProjectTaskDto.Id = id;

            var projectTask = await _projectTaskService.UpdateProjectTaskAsync(updateProjectTaskDto);

            return Ok(projectTask);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProjectTask(int id)
        {
            await _projectTaskService.DeleteProjectTaskAsync(id);

            return Ok();
        }

        [HttpGet("find")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsedProduct([FromQuery] int productId, [FromQuery] PaginationParameters paginationParameters)
        {
            var projectTask = await _projectTaskService.GetUsedProductAsync(productId, paginationParameters);

            return Ok(projectTask);
        }
    }
}
