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

        bool AddUpdateWorker(Workers worker);

        bool DeleteWorker(Guid id);

        List<Workers> GetWorkers();

        List<PendingSubmissions> GetPendingSubmissions();

        bool AddOfflineSubmission(Jobs jobs);

        bool RemoveOfflineSubmission(Guid id);

        List<string> GetUploadedAssembliesList();

        bool UploadAssembly(byte[] assemblyBytes);

        Guid? GetUser(string username, string password);

        Guid? CreateUser(string username, string password);

        List<Users> GetUsers();

        void RecordLogin(Guid? userID, string username, string ipAddress, bool successful);
    }
}