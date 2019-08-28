using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMTP.REST.Models.Users
{
    public class CreateEditUserModel
    {
        public Guid? ID { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string SelectedRole { get; set; }

        public List<SelectListItem> Roles { get; set; }

        public string Message { get; set; }
    }
}