using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
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

        public bool AddUpdateHost(Hosts host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (!host.IsValid())
            {
                throw new ValidationException("Not all required fields are set");
            }

            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    host.LastConnected = DateTime.Now;

                    var dbHost = db.GetCollection<Hosts>().FindOne(a => a.Name == host.Name);

                    if (dbHost == null)
                    {
                        db.GetCollection<Hosts>().Insert(host);
                    }
                    else
                    {
                        host.ID = dbHost.ID;

                        db.GetCollection<Hosts>().Update(host);
                    }

                    return true;
                }
            } catch (Exception ex)
            {
                Log.Error($"Failed to Add or Update Host {host} due to {ex}");

                return false;
            }
        }

        public bool DeleteHost(Guid id)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    db.GetCollection<Hosts>().Delete(a => a.ID == id);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Delete Host {id} due to {ex}");

                return false;
            }
        }

        public List<Hosts> GetHosts()
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    return db.GetCollection<Hosts>().FindAll().ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Get Hosts due to {ex}");

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

        public Guid? GetUser(string username, string password)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    var user = db.GetCollection<Users>().FindOne(a => a.Username == username && a.Password == password);

                    return user?.ID;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get user due to {ex}");

                return null;
            }
        }
    }
}