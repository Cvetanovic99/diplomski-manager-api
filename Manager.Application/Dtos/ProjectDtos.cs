using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Manager.Core.Entities;

namespace Manager.Application.Dtos
{
    public class ProjectDto
    {
        [Required] 
        public int Id { get; set; }
        
        [Required]
        public string State { get; set; }

        [Required]
        public ClientDto Client { get; set; }

        [Required]
        public WarehouseDto Warehouse { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string Title { get; set; }

        public PdfFileDto PdfFile { get; set; }
    }
    
    public class ProjectExcelDto
    {
        [Required] 
        public int Id { get; set; }
        
        [Required] 
        public string Title { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public ClientDto Client { get; set; }

        [Required]
        public WarehouseDto Warehouse { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        [Required]
        public ICollection<ProjectTaskDto> Tasks { get; set; }
    }

    public class CreateProjectDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        public string State { get; set; }

        [Required]
        public string Title { get; set; }
    }
    
    public class UpdateProjectDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public int ClientId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Title { get; set; }
    }
}