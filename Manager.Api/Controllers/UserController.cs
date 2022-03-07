using Manager.Application.Dtos;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Manager.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly IProductService _productService;
        private readonly IFileManagerService _fileManagerService;

        public UserController(IUserService userService, IAccountService accountService, IProductService productService, IFileManagerService fileManagerService)
        {
            _userService = userService;
            _accountService = accountService;
            _fileManagerService = fileManagerService;
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployees([FromQuery]PaginationParameters paginationParameters)
        {
            var employyes = await _userService.GetUsersAsync(paginationParameters);

            return Ok(employyes);
        }
        
        [HttpGet]
        [Route("{userId}/colleagues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserColleagues(int userId, [FromQuery]PaginationParameters paginationParameters)
        {
            var colleagues = await _userService.GetUserColleaguesAsync(userId, paginationParameters);

            return Ok(colleagues);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployee(int id)
        {
                var user = await _userService.GetUserByIdAsync(id);

                return Ok(user);
        }


        [HttpGet]
        [Route("projects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployeeProjects([FromQuery] ProjectPaginationParameters paginationParameters)
        {
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                var userId = await _accountService.GetIdForAuthenticatedUserAsync(token);

                var projects = await _userService.GetUserProjectsAsync(userId, paginationParameters);

                return Ok(projects);
            }
            else
            {
                return NotFound("Token does not exist.");
            }
        }
        
        [HttpGet]
        [Route("{id}/web-report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Excel(int id, [FromQuery] ReportQueryParameters reportQueryParameters)
        {
            var projects = await _userService.GetProjectsWithTasksBetweenDatesExcelAsync(id, reportQueryParameters);

            return Ok(projects);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            updateUserDto.Id = id;

            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {

                await _userService.UpdateUserAsync(updateUserDto, token);
                return Ok();
            }
        
            return NotFound("Token does not exist.");
        }

        [HttpPut("{id}/tools")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserTools(int id, UpdateUserToolsDto updateUserToolsDto)
        {
            updateUserToolsDto.Id = id;

            await _userService.UpdateUserToolsAsync(updateUserToolsDto);
            
            return Ok();
        }

        [HttpGet]
        [Route("{id}/report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployeeReport(int id, [FromQuery] ReportQueryParameters reportQueryParameters)
        {
            var excelModel = await _userService.GetProjectsWithTasksBetweenDatesExcelAsync(id, reportQueryParameters);
            var allProducts = await _productService.GetAllProductsAsync();

            var reportDocument = _fileManagerService.GetEmployeeReportAsExcelDocument(excelModel, allProducts);

            return Ok(JsonSerializer.Serialize(reportDocument));
        }
        
        [HttpGet]
        [Route("{id}/all-projects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEmployeeProjects(int id)
        {
            var projects = await _userService.GetAllUserProjectsAsync(id);

            return Ok(projects);
        }
        
        [HttpPut("{id}/password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserPassword(int id, PasswordDto passwordDto)
        {
            await _userService.SetEmployeePassword(id, passwordDto);
            
            return Ok();
        }
    }
}
