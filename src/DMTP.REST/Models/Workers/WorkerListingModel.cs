using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DMTP.REST.Models.Workers
{
    public class WorkerListingModel
    {
        public List<lib.dal.Databases.Tables.Workers> WorkersListing { get; set; }

        public string RegistrationKey { get; set; }

        public string WebServiceURL { get; set; }
    }
}