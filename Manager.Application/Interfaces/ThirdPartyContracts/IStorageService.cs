using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Manager.Application.Dtos;

namespace Manager.Application.Interfaces.ThirdPartyContracts
{
    public interface IStorageService
    {
        Task<PdfFileDto> UploadAsync(IFormFile formFile, string nameWithoutExtension = null);
        Task DeleteAsync(string filePath); 
        string GetFile(string filePath);
    }
}