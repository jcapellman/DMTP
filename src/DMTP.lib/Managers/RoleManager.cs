using System;
using System.Collections.Generic;

using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Enums;
using DMTP.lib.Enums;
using DMTP.lib.Managers.Base;

namespace DMTP.lib.Managers
{
    public class RoleManager : BaseManager
    {
        public RoleManager(IDatabase database) : base(database)
        {
        }

        public List<Roles> GetRoles()
        {
            try
            {
                return _database.GetAll<Roles>();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get roles due to {ex}");

                return null;
            }
        }

        public Guid? CreateRole(string name, bool builtIn, Dictionary<AccessSections, AccessLevels> permissions)
        {
            try
            {
                var item = new Roles
                {
                    Name = name,
                    Active = true,
                    BuiltIn = builtIn,
                    Permissions = permissions
                };

                return _database.Insert<Roles>(item);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to insert roles due to {ex}");

                return null;
            }
        }

        public bool UpdateRole(Guid id, string name, Dictionary<AccessSections, AccessLevels> permissions)
        {
            try
            {
                var dbRole = _database.GetOneById<Roles>(id);

                if (dbRole == null)
                {
                    return false;
                }

                dbRole.Name = name;
                dbRole.Permissions = permissions;

                _database.Update<Roles>(dbRole);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to update role {id} in the Roles Table to {ex}");

                return false;
            }
        }

        public bool DeleteRole(Guid roleID)
        {
            try
            {
                var role = _database.GetOneById<Roles>(roleID);

                if (role == null)
                {
                    return false;
                }

                role.Active = false;

                return _database.Update<Roles>(role);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to set {roleID} to in active from the Roles Table to {ex}");

                return false;
            }
        }
    }
}