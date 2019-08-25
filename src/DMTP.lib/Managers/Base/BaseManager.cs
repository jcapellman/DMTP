using DMTP.lib.dal.Databases.Base;

namespace DMTP.lib.Managers.Base
{
    public class BaseManager
    {
        protected readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        protected IDatabase _database;

        public BaseManager(IDatabase database)
        {
            _database = database;
        }
    }
}