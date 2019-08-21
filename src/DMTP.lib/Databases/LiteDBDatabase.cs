using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DMTP.lib.Auth;
using DMTP.lib.Common;
using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;
using DMTP.lib.Helpers;
using DMTP.lib.ML.Base;

using LiteDB;

using Newtonsoft.Json;

namespace DMTP.lib.Databases
{
    public class LiteDBDatabase : IDatabase
    {
        private readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private const string DbFilename = "data.db";

        public bool DeleteJob(Guid id)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Jobs>().Delete(a => a.ID == id) > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Delete Job ({id}) due to {ex}");

                return false;
            }
        }

        public bool AddJob(Jobs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.IsValid())
            {
                throw new ValidationException("Not all required fields are set");
            }

            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Jobs>().Insert(item) != null;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Add Job ({item}) due to {ex}");

                return false;
            }
        }

        public bool UpdateJob(Jobs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.IsValid())
            {
                throw new ValidationException("Not all required fields are set");
            }

            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Jobs>().Update(item);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Update Job ({item}) due to {ex}");

                return false;
            }
        }

        public Jobs GetJob(Guid id)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Jobs>().FindOne(a => a.ID == id);
                }
            } catch (Exception ex)
            {
                Log.Error($"Failed to obtain Job from {id} due to: {ex}");

                return null;
            }
        }

        public List<Jobs> GetJobs()
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Jobs>().FindAll().ToList();
                }
            } catch (Exception ex)
            {
                Log.Error($"Failed to obtain jobs due to {ex}");

                return null;
            }
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
                using (var db = new LiteDatabase(DbFilename))
                {
                    worker.LastConnected = DateTime.Now;

                    var dbHost = db.GetCollection<Workers>().FindOne(a => a.Name == worker.Name);

                    if (dbHost == null)
                    {
                        db.GetCollection<Workers>().Insert(worker);
                    }
                    else
                    {
                        worker.ID = dbHost.ID;

                        db.GetCollection<Workers>().Update(worker);
                    }

                    return true;
                }
            } catch (Exception ex)
            {
                Log.Error($"Failed to Add or Update Worker {worker} due to {ex}");

                return false;
            }
        }

        public bool DeleteWorker(Guid id)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    db.GetCollection<Workers>().Delete(a => a.ID == id);

                    return true;
                }
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
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Workers>().FindAll().ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Get Workers due to {ex}");

                return null;
            }
        }

        public List<PendingSubmissions> GetPendingSubmissions()
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<PendingSubmissions>().FindAll().ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Get Pending Submissions due to {ex}");

                return null;
            }
        }

        public bool AddOfflineSubmission(Jobs job)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    var pendingSubmission = new PendingSubmissions
                    {
                        ID = job.ID,
                        JobJSON = JsonConvert.SerializeObject(job)
                    };

                    db.GetCollection<PendingSubmissions>().Insert(pendingSubmission);

                    return true;
                }
            } catch (Exception ex)
            {
                Log.Error($"Failed to Add Offline Submission {job} due to {ex}");

                return false;
            }
        }

        public bool RemoveOfflineSubmission(Guid id)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    db.GetCollection<PendingSubmissions>().Delete(a => a.ID == id);

                    return true;
                }
            } catch (Exception ex)
            {
                Log.Error($"Failed to remove {id} from the PendingSubmissions due to {ex}");

                return false;
            }
        }

        public List<string> GetUploadedAssembliesList()
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Assemblies>().FindAll().Select(a => a.Name).ToList();
                }
            } catch (Exception ex)
            {
                Log.Error($"Failed to get uploaded assemblies list due to {ex}");

                return null;
            }
        }

        public bool UploadAssembly(byte[] assemblyBytes)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    if (assemblyBytes == null)
                    {
                        throw new ArgumentNullException(nameof(assemblyBytes));
                    }

                    var assembly = Assembly.Load(assemblyBytes);

                    if (assembly == null)
                    {
                        throw new ArgumentException("File uploaded is not a valid DLL");
                    }

                    var baseExtractorType = assembly.DefinedTypes.FirstOrDefault(a => a.BaseType == typeof(BasePrediction));

                    if (baseExtractorType == null)
                    {
                        throw new ArgumentException("File was a valid DLL, but was not compiled properly");
                    }

                    var baseExtractor = (BasePrediction)Activator.CreateInstance(baseExtractorType);

                    var item = new Assemblies
                    {
                        Name = baseExtractor.MODEL_NAME,
                        Data = assemblyBytes
                    };

                    db.GetCollection<Assemblies>().Insert(item);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failure to upload assembly {ex.StackTrace}");

                return false;
            }
        }

        public Users GetUser(Guid id)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Users>().FindOne(a => a.ID == id && a.Active);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get user due to {ex}");

                return null;
            }
        }

        public Guid? GetUser(string username, string password)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    var user = db.GetCollection<Users>().FindOne(a => a.EmailAddress == username && a.Password == password && a.Active);

                    return user?.ID;
                }
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
                using (var db = new LiteDatabase(DbFilename))
                {
                    var user = db.GetCollection<Users>().FindOne(a => a.EmailAddress == emailAddress);

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

                    return db.GetCollection<Users>().Insert(user);
                }
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
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Users>().FindAll().Where(a => a.Active).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get users due to {ex}");

                return null;
            }
        }

        public void RecordLogin(Guid? userID, string username, string ipAddress, bool successful)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    var item = new UserLogins
                    {
                        IPAddress = ipAddress,
                        Successful = successful,
                        UserID = userID,
                        Timestamp = DateTimeOffset.Now,
                        Username = username
                    };

                    db.GetCollection<UserLogins>().Insert(item);
                }
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
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<UserLogins>().FindAll().ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get user logins due to {ex}");

                return null;
            }
        }

        public bool DeleteUser(Guid userID)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    var user = db.GetCollection<Users>().FindById(userID);

                    if (user == null)
                    {
                        return false;
                    }

                    user.Active = false;

                    db.GetCollection<Users>().Update(user);

                    return true;
                }
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
                using (var db = new LiteDatabase(DbFilename))
                {
                    var dbUser = db.GetCollection<Users>().FindById(user.ID);

                    if (dbUser == null)
                    {
                        return false;
                    }

                    dbUser.FirstName = user.FirstName;
                    dbUser.LastName = user.LastName;

                    db.GetCollection<Users>().Update(dbUser);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to update user {user.ID} in the Users Table to {ex}");

                return false;
            }
        }

        public Settings GetSettings()
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    var settings = db.GetCollection<Settings>().FindOne(a => a != null);

                    if (settings != null)
                    {
                        return settings;
                    }

                    settings = new Settings
                    {
                        AllowNewUserCreation = false,
                        SMTPHostName = string.Empty,
                        SMTPPassword = string.Empty,
                        SMTPPortNumber = 467,
                        SMTPUsername = string.Empty,
                        IsInitialized = false
                    };

                    db.GetCollection<Settings>().Insert(settings);

                    return settings;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get settings due to {ex}");

                return null;
            }
        }

        public bool UpdateSettings(Settings setting)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    db.GetCollection<Settings>().Update(setting);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to update settings {setting.ID} in the Settings Table to {ex}");

                return false;
            }
        }

        public List<Roles> GetRoles()
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Roles>().FindAll().ToList();
                }
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
                using (var db = new LiteDatabase(DbFilename))
                {
                    var item = new Roles
                    {
                        Name = name,
                        Active = true,
                        BuiltIn = builtIn,
                        Permissions = permissions
                    };

                    return db.GetCollection<Roles>().Insert(item);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to insert roles due to {ex}");

                return null;
            }
        }

        public bool DeleteRole(Guid roleID)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    var role = db.GetCollection<Roles>().FindById(roleID);

                    if (role == null)
                    {
                        return false;
                    }

                    role.Active = false;

                    db.GetCollection<Roles>().Update(role);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to set {roleID} to in active from the Roles Table to {ex}");

                return false;
            }
        }

        public void Initialize()
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    // Delete all rows
                    db.GetCollection<Users>().Delete(a => a != null);

                    db.GetCollection<Roles>().Delete(a => a != null);

                    db.GetCollection<Workers>().Delete(a => a != null);

                    db.GetCollection<Assemblies>().Delete(a => a != null);

                    db.GetCollection<Jobs>().Delete(a => a != null);

                    db.GetCollection<PendingSubmissions>().Delete(a => a != null);

                    db.GetCollection<Settings>().Delete(a => a != null);

                    db.GetCollection<UserLogins>().Delete(a => a != null);

                    CreateRole(Constants.ROLE_BUILTIN_ADMIN, true,
                        Enum.GetNames(typeof(AccessSections))
                            .ToDictionary(Enum.Parse<AccessSections>, section => AccessLevels.FULL));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Initialize due to {ex}");
            }
        }

        public ApplicationUser GetApplicationUser(Guid userID)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    var user = db.GetCollection<Users>().FindById(userID);

                    if (user == null)
                    {
                        throw new Exception("Could not retrieve user");
                    }

                    return new ApplicationUser
                    {
                        ID = userID,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = GetRoles().FirstOrDefault(a => a.ID == user.RoleID)
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get information of {userID} due to {ex}");

                return null;
            }
        }
    }
}