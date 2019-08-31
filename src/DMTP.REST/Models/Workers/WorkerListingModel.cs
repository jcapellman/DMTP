using System.Collections.Generic;

namespace DMTP.REST.Models.Workers
{
    public class WorkerListingModel
    {
        public List<lib.dal.Databases.Tables.Workers> WorkersListing { get; set; }

        public string RegistrationKey { get; set; }
    }
}