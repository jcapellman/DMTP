﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.ML.Base;

using LiteDB;

using Newtonsoft.Json;
using Logger = NLog.Logger;

namespace DMTP.lib.Databases
{
    public class LiteDBDatabase : IDatabase
    {
        private Logger Log = NLog.LogManager.GetCurrentClassLogger();

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
            using (var db = new LiteDatabase(DbFilename))
            {
                return db.GetCollection<Jobs>().FindOne(a => a.ID == id);
            }
        }

        public List<Jobs> GetJobs()
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                return db.GetCollection<Jobs>().FindAll().ToList();
            }
        }

        public void AddUpdateHost(Hosts host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (!host.IsValid())
            {
                throw new ValidationException("Not all required fields are set");
            }

            using (var db = new LiteDatabase(DbFilename))
            {
                host.LastConnected = DateTime.Now;

                var dbHost = db.GetCollection<Hosts>().FindOne(a => a.Name == host.Name);

                if (dbHost == null)
                {
                    db.GetCollection<Hosts>().Insert(host);
                } else
                {
                    host.ID = dbHost.ID;

                    db.GetCollection<Hosts>().Update(host);
                }
            }
        }

        public void DeleteHost(Guid id)
        {
            try
            {
                using (var db = new LiteDatabase(DbFilename))
                {
                    db.GetCollection<Hosts>().Delete(a => a.ID == id);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to Delete Host {id} due to {ex}");
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

        public void AddOfflineSubmission(Jobs job)
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                var pendingSubmission = new PendingSubmissions
                {
                    ID = job.ID,
                    JobJSON = JsonConvert.SerializeObject(job)
                };

                db.GetCollection<PendingSubmissions>().Insert(pendingSubmission);
            }
        }

        public void RemoveOfflineSubmission(Guid id)
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                db.GetCollection<PendingSubmissions>().Delete(a => a.ID == id);
            }
        }

        public List<string> GetUploadedAssembliesList()
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                return db.GetCollection<Assemblies>().FindAll().Select(a => a.Name).ToList();
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
    }
}