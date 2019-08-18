using System.ComponentModel.DataAnnotations;

namespace DMTP.REST.Models.Setup
{
    public class SetupModel
    {
        public string ActionMessage { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string SMTPHostName { get; set; }

        [Required]
        public string SMTPUsername { get; set; }

        [Required]
        public string SMTPPassword { get; set; }

        [Required]
        public int SMTPPortNumber { get; set; }

        [Required]
        public bool AllowUserCreation { get; set; }
    }
}