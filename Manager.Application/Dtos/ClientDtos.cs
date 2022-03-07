using System.ComponentModel.DataAnnotations;

namespace Manager.Application.Dtos
{
    
    public class ClientDto
    {
        [Required] 
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string ClientId { get; set; }
        
        public string Email { get; set; }
        
        public string Address { get; set; }

        public string PhoneNumber { get; set; }
        
        public string City { get; set; }
    }

    public class CreateClientDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string ClientId { get; set; }
        
        public string Email { get; set; }
        
        public string Address { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string City { get; set; }
    }

    public class UpdateClientDto : ClientDto
    {
            
    }
}