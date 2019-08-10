using DMTP.lib.Databases.Tables.Base;

namespace DMTP.lib.Databases.Tables
{
    public class Users : BaseTable
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}