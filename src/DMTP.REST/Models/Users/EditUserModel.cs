using System;

namespace DMTP.REST.Models.Users
{
    public class EditUserModel
    {
        public Guid ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Message { get; set; }
    }
}