using System.ComponentModel.DataAnnotations;
using Manager.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Manager.Application.Dtos
{
    public class PdfFileDto
    {
        public int Id { get; set; }
        
        public string Path { get; set; }

        public string Name { get; set; }

        //public Project Project { get; set; }
    }

    public class CreatePdfFileDto
    {
        public string Path { get; set; }
        
        public string Name { get; set; }

        [Required]
        public int ProjectId { get; set; }
        
        [Required]
        public IFormFile File { get; set; }
    }

    public class DeletePdfFileDto
    {
        [Required]
        public string FilePath { get; set; }
    }
    
    public class DownloadPdfFileDto
    {
        [Required]
        public string FilePath { get; set; }
    }
    
    

}