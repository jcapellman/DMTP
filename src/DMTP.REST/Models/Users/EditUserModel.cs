using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMTP.REST.Models.Users
{
    public class EditUserModel
    {
        public Guid ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SelectedRole { get; set; }

        public List<SelectListItem> Roles { get; set; }

        public string Message { get; set; }
    }
}