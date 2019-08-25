using DMTP.lib.dal.Manager;

namespace DMTP.lib.Managers.Base
{
    public class BaseManager
    {
        protected readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        protected DatabaseManager _database;

        public BaseManager(DatabaseManager database)
        {
            _database = database;
        }
    }
}