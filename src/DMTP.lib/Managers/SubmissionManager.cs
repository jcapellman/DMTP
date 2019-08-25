using System;
using System.Collections.Generic;

using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.Managers.Base;

using Newtonsoft.Json;

namespace DMTP.lib.Managers
{
    public class SubmissionManager : BaseManager
    {
        public SubmissionManager(IDatabase database) : base(database)
        {
        }

        public List<PendingSubmissions> GetPendingSubmissions()
        {
            try
            {
                return _database.GetAll<PendingSubmissions>();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Get Pending Submissions due to {ex}");

                return null;
            }
        }

        public bool AddOfflineSubmission(Jobs job)
        {
            try
            {
                var pendingSubmission = new PendingSubmissions
                {
                    ID = job.ID,
                    JobJSON = JsonConvert.SerializeObject(job)
                };

                _database.Insert(pendingSubmission);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Add Offline Submission {job} due to {ex}");

                return false;
            }
        }

        public bool RemoveOfflineSubmission(Guid id)
        {
            try
            {
                return _database.Delete<PendingSubmissions>(id);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to remove {id} from the PendingSubmissions due to {ex}");

                return false;
            }
        }
    }
}