using System.Threading.Tasks;
using Manager.Application.Dtos;

namespace Manager.Application.Interfaces
{
    public interface IPdfFileService
    {
        Task DeleteAsync(string filePath);
        Task<PdfFileDto> CreateAsync(CreatePdfFileDto createPdfFileDto);
        string DownloadPdfFile(string filePath);
    }
}