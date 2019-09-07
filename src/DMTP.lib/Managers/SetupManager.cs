using System;
using System.Linq;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Enums;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers.Base;

namespace DMTP.lib.Managers
{
    public class SetupManager : BaseManager
    {
        public SetupManager(DatabaseManager database) : base(database)
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

                var permissions = Enum.GetNames(typeof(AccessSections)).ToDictionary(Enum.Parse<AccessSections>, section => AccessLevels.FULL);

                new RoleManager(_database).CreateRole(Common.Constants.ROLE_BUILTIN_ADMIN, true, permissions);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Initialize due to {ex}");
            }
        }
    }
}