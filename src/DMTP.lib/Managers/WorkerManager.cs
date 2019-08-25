using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers.Base;

namespace DMTP.lib.Managers
{
    public class WorkerManager : BaseManager
    {
        public WorkerManager(DatabaseManager database) : base(database)
        {
        }

        public bool AddUpdateWorker(Workers worker)
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            if (!worker.IsValid())
            {
                throw new ValidationException("Not all required fields are set");
            }

            try
            {
                worker.LastConnected = DateTime.Now;

                var dbHost = _database.GetOne<Workers>(a => a.Name == worker.Name);

                if (dbHost == null)
                {
                    _database.Insert(worker);
                }
                else
                {
                    worker.ID = dbHost.ID;

                    _database.Update(worker);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Add or Update Worker {worker} due to {ex}");

                return false;
            }
        }

        public bool DeleteWorker(Guid id)
        {
            try
            {
                return _database.Delete<Workers>(id);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Delete Worker {id} due to {ex}");

                return false;
            }
        }

        public List<Workers> GetWorkers()
        {
            try
            {
                return _database.GetAll<Workers>();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Get Workers due to {ex}");

                return null;
            }
        }
    }
}