using System.ComponentModel.DataAnnotations;

namespace DMTP.REST.Models
{
    public class LoginViewModel
    {
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public lib.dal.Databases.Tables.Settings CurrentSettings { get; set; }
    }
}