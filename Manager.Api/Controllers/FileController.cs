using System.Text.Json;
using System.Threading.Tasks;
using Manager.Application.Dtos;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IPdfFileService _pdfFileService;

        public FileController(IStorageService storageService, IPdfFileService pdfFileService)
        {
            this._storageService = storageService;
            this._pdfFileService = pdfFileService;
        }

        [HttpPost("pdf-upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] CreatePdfFileDto createPdfFileDto)
        {

            var pdfFileDto = await _storageService.UploadAsync(createPdfFileDto.File);

            createPdfFileDto.Path = pdfFileDto.Path;
            createPdfFileDto.Name = pdfFileDto.Name;
            var pdfFile = await _pdfFileService.CreateAsync(createPdfFileDto);

            return Ok(pdfFile); 
        }
        
        [HttpDelete("pdf-delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromBody]DeletePdfFileDto deletePdfFileDto)
        {
            await _storageService.DeleteAsync(deletePdfFileDto.FilePath);
            await _pdfFileService.DeleteAsync(deletePdfFileDto.FilePath);

            return NoContent();
        }
        
        [HttpGet]
        [Route("pdf-download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Downloadint ([FromQuery] DownloadPdfFileDto downloadPdfFileDto)
        {
            var pdfFile = _pdfFileService.DownloadPdfFile(downloadPdfFileDto.FilePath);
            return Ok(JsonSerializer.Serialize(pdfFile));
        }
    }
}