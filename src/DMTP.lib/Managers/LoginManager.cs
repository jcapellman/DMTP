using System;
using System.Collections.Generic;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers.Base;

namespace DMTP.lib.Managers
{
    public class LoginManager : BaseManager
    {
        public LoginManager(DatabaseManager database) : base(database)
        {
        }

        public void RecordLogin(Guid? userID, string username, string ipAddress, bool successful)
        {
            try
            {
                var item = new UserLogins
                {
                    IPAddress = ipAddress,
                    Successful = successful,
                    UserID = userID,
                    Timestamp = DateTimeOffset.Now,
                    Username = username
                };

                _database.Insert(item);
            
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to log login due to {ex}");
            }
        }

        public List<UserLogins> GetLogins()
        {
            try
            {
                return _database.GetAll<UserLogins>();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get user logins due to {ex}");

                return null;
            }
        }
    }
}