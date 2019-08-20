using System;
using System.ComponentModel.DataAnnotations;

namespace DMTP.REST.Models
{
    public class CreateUserModel
    {
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public Guid RoleID { get; set; }
    }
}