using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;

using DMTP.lib.Managers.Base;

namespace DMTP.lib.Managers
{
    public class JobManager : BaseManager
    {
        public JobManager(DatabaseManager database) : base(database)
        {
        }

        public bool DeleteJob(Guid id)
        {
            try
            {
                return _database.Delete<Jobs>(id);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Delete Job ({id}) due to {ex}");

                return false;
            }
        }

        public bool AddJob(Jobs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.IsValid())
            {
                throw new ValidationException("Not all required fields are set");
            }

            try
            {
                _database.Insert<Jobs>(item);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Add Job ({item}) due to {ex}");

                return false;
            }
        }

        public bool UpdateJob(Jobs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.IsValid())
            {
                throw new ValidationException("Not all required fields are set");
            }

            try
            {
                return _database.Update<Jobs>(item);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Update Job ({item}) due to {ex}");

                return false;
            }
        }

        public Jobs GetJob(Guid id)
        {
            try
            {
                return _database.GetOneById<Jobs>(id);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to obtain Job from {id} due to: {ex}");

                return null;
            }
        }

        public List<Jobs> GetJobs()
        {
            try
            {
                return _database.GetAll<Jobs>();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to obtain jobs due to {ex}");

                return null;
            }
        }
    }
}