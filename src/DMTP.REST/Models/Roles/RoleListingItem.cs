using System;

namespace DMTP.REST.Models.Roles
{
    public class RoleListingItem
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public int NumberAssociatedUsers { get; set; }

        public bool IsBuiltIn { get; set; }
    }
}