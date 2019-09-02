using System.Collections.Generic;

namespace DMTP.REST.Models.Assemblies
{
    public class AssemblyDashboardModel
    {
        public List<lib.dal.Databases.Tables.Assemblies> AssembliesList { get; set; }

        public string ActionMessage { get; set; }
    }
}