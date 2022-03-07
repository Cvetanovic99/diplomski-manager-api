using System;
using System.ComponentModel.DataAnnotations;

namespace Manager.Application.Dtos
{
    public class ProjectTaskDto
    {
        public int Id { get; set; }
        public float QuantityUsed { get; set; }
        public string Description { get; set; }
        
        public string SN { get; set; }
        
        public ProductStateDto ProductState { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public ProjectDto Project { get; set; }

        [Required]
        public UserDto Employed1 { get; set; }
        
        public UserDto Employed2 { get; set; }
    }

    public class CreateProjectTaskDto
    {
        [Required]
        public float QuantityUsed { get; set; }

        public string Description { get; set; }

        public int? ProductStateId { get; set; }
        
        public string SN { get; set; }

        [Required]
        public int ProjectId { get; set; }
        
        [Required]
        public int Employed1Id { get; set; }
        
        public int? Employed2Id { get; set; }
    }

    public class UpdateProjectTaskDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public float QuantityUsed { get; set; }

        public string Description { get; set; }

        public int? ProductStateId { get; set; }
        
        public string SN { get; set; }
        
        [Required]
        public int ProjectId { get; set; }
        
        [Required]
        public int Employed1Id { get; set; }
        
        public int? Employed2Id { get; set; }
    }

    public class ProjectTaskWithProductDto : ProjectTaskDto
    {
        public ProductDto Product { get; set; }
    }


}