using System;
using System.Collections.Generic;

using DMTP.lib.Auth;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers.Base;

namespace DMTP.lib.Managers
{
    public class UserManager : BaseManager
    {
        public UserManager(DatabaseManager database) : base(database)
        {
        }

        public ApplicationUser GetApplicationUser(Guid userID)
        {
            try
            {
                var user = _database.GetOneById<Users>(userID);

                if (user == null)
                {
                    throw new Exception("Could not retrieve user");
                }

                return new ApplicationUser
                {
                    ID = userID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = _database.GetOne<Roles>(a => a.ID == user.RoleID)
                };
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get information of {userID} due to {ex}");

                return null;
            }
        }


        public Users GetUser(Guid id)
        {
            try
            {
                return _database.GetOneById<Users>(id);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get user due to {ex}");

                return null;
            }
        }

        public bool DeleteUser(Guid userID)
        {
            try
            {
                var user = _database.GetOneById<Users>(userID);

                if (user == null)
                {
                    return false;
                }

                user.Active = false;

                _database.Update(user);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to set {userID} to in active from the Users Table to {ex}");

                return false;
            }
        }

        public bool UpdateUser(Users user)
        {
            try
            {
                var dbUser = _database.GetOneById<Users>(user.ID);

                if (dbUser == null)
                {
                    return false;
                }

                dbUser.FirstName = user.FirstName;
                dbUser.LastName = user.LastName;

                _database.Update<Users>(dbUser);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to update user {user.ID} in the Users Table to {ex}");

                return false;
            }
        }

        public Guid? GetUser(string username, string password)
        {
            try
            {
                var user = _database.GetOne<Users>(a => a.EmailAddress == username && a.Password == password && a.Active);

                return user?.ID;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get user due to {ex}");

                return null;
            }
        }

        public Guid? CreateUser(string emailAddress, string firstName, string lastName, string password, Guid roleID)
        {
            try
            {
                var user = _database.GetOne<Users>(a => a.EmailAddress == emailAddress && a.Password == password && a.Active);

                if (user != null)
                {
                    return null;
                }

                user = new Users
                {
                    EmailAddress = emailAddress,
                    FirstName = firstName,
                    LastName = lastName,
                    Password = password,
                    Active = true,
                    RoleID = roleID
                };

                return _database.Insert<Users>(user);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create user due to {ex}");

                return null;
            }
        }

        public List<Users> GetUsers()
        {
            try
            {
                return _database.GetAll<Users>();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get users due to {ex}");

                return null;
            }
        }
    }
}