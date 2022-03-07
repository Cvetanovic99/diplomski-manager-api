using System;
using System.ComponentModel.DataAnnotations;

namespace Manager.Application.Dtos
{
    public class ProductStateDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public float Quantity { get; set; }
        
        public string SN { get; set; }
        
        public int ProductId { get; set; }

        public WarehouseDto Warehouse { get; set; }

        public string ProductName { get; set; }
    }

    public class UpdateProductStateDto
    {   [Required] 
        public int ProductId { get; set; }
        
        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public float Quantity { get; set; }

        public string SNCodes { get; set; }
    }
    
    
}