using System.Linq;

using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;

namespace DMTP.lib.Extensions
{
    public static class RolesExtensions
    {
        public static bool HasPermissions(this Roles role, AccessSections section, AccessLevels level) => role.Permissions.Any(a => a.Key == section && a.Value >= level);
    }
}