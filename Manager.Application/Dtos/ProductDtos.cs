using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Manager.Application.Dtos
{
    public class ProductDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Manufacturer { get; set; }
        
        [Required]
        public string Supplier { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool HasSN { get; set; }
        
        [Required]
        public string Model { get; set; }
        
        [Required]
        public string Unit { get; set; }
        
        public IList<ProductStateDto> States { get; set; }

    }

    public class CreateProductDto
    {
        [Required]
        public string Manufacturer { get; set; }
        
        [Required]
        public string Supplier { get; set; }
        
        [Required]
        public bool HasSN { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Model { get; set; }
        
        [Required]
        public string Unit { get; set; }

    }

    public class UpdateProductDto : ProductDto
    {
        
    }

    public class ProductWarehouseDto : ProductDto
    {
        
    }
}