using System.ComponentModel.DataAnnotations;

namespace DMTP.REST.Models
{
    public class CreateUserModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
    }
}