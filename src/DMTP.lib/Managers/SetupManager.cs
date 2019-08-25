using System;

using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.Managers.Base;

namespace DMTP.lib.Managers
{
    public class SetupManager : BaseManager
    {
        public SetupManager(IDatabase database) : base(database)
        {
        }

        public void Initialize()
        {
            try
            {
                _database.DeleteAll<Users>();
                _database.DeleteAll<Roles>();
                _database.DeleteAll<Workers>();
                _database.DeleteAll<Assemblies>();
                _database.DeleteAll<Jobs>();
                _database.DeleteAll<PendingSubmissions>();
                _database.DeleteAll<Settings>();
                _database.DeleteAll<UserLogins>();
                /*
                CreateRole(Constants.ROLE_BUILTIN_ADMIN, true,
                    Enum.GetNames(typeof(AccessSections))
                        .ToDictionary(Enum.Parse<AccessSections>, section => AccessLevels.FULL));*/
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Initialize due to {ex}");
            }
        }
    }
}