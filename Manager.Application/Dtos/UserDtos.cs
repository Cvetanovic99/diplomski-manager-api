using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Manager.Application.Dtos
{
    public class UserDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public string IdentityId { get; set; }

        public string Tools { get; set; }

        public string Email { get; set; }
    }

    public class CreateUserDto
    { 
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string IdentityId { get; set; }

        public string Tools { get; set; }
    }

    public class UpdateUserDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Email { get; set; }

        public string Tools { get; set; }

    }

    public class UserWithEmailDto : UserDto
    {
    }

    public class UpdateUserToolsDto
    {
        public int Id { get; set; }
        public string Tools { get; set; }
       
    }
}
