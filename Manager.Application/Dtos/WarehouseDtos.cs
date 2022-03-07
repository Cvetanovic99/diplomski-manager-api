using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Manager.Application.Dtos
{
    public class WarehouseDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string City { get; set; }
    }

    public class WarehouseProductDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string City { get; set; }

        [Required]
        public IList<ProductStateDto> States { get; set; }

    }

    public class CreateWarehouseDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string City { get; set; }
    }

    public class UpdateWarehouseDto : WarehouseDto
    {
        
    }

    public class AddProductToWarehouse 
    {
        [Required]
        public float Quantity { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }

        public string SNCodes { get; set; }
    }
}
