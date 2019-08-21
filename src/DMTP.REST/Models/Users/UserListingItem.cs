using System;

namespace DMTP.REST.Models.Users
{
    public class UserListingItem
    {
        public Guid ID { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset? LastLogin { get; set; }

        public int NumJobs { get; set; }
    }
}