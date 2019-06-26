using System;
using System.Collections.Generic;

using DMTP.lib.Databases.Tables;

namespace DMTP.lib.Databases.Base
{
    public interface IDatabase
    {
        bool DeleteJob(Guid id);

        bool AddJob(Jobs item);

        bool UpdateJob(Jobs item);

        Jobs GetJob(Guid id);

        List<Jobs> GetJobs();

        void AddUpdateHost(Hosts host);

        void DeleteHost(Guid id);

        List<Hosts> GetHosts();

        List<PendingSubmissions> GetPendingSubmissions();

        void AddOfflineSubmission(Jobs jobs);

        void RemoveOfflineSubmission(Guid id);

        List<string> GetUploadedAssembliesList();

        bool UploadAssembly(byte[] assemblyBytes);
    }
}