using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Databases.Tables.Base;

using LiteDB;

namespace DMTP.lib.dal.Databases
{
    public class LiteDBDatabase : IDatabase
    {
        private const string DbFilename = "data.db";

        public Guid Insert<T>(T objectValue) where T : BaseTable
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                objectValue.Active = true;

                return db.GetCollection<T>().Insert(objectValue);
            }
        }

        public bool InsertRange<T>(List<T> objects) where T : BaseTable
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                db.GetCollection<T>().Insert(objects);

                return true;
            }
        }

        public bool DeleteAll<T>() where T : BaseTable
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                return db.DropCollection(nameof(T));
            }
        }

        public bool Delete<T>(Guid id) where T : BaseTable
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                return db.GetCollection<T>().Delete(id);
            }
        }

        public T GetOneById<T>(Guid id) where T : BaseTable
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                return db.GetCollection<T>().FindOne(a => a.ID == id && a.Active);
            }
        }

        public T GetOne<T>(Expression<Func<T, bool>> query) where T : BaseTable
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                return db.GetCollection<T>().FindOne(query);
            }
        }

        public List<T> GetAll<T>() where T : BaseTable
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                return db.GetCollection<T>().Find(a => a.Active).ToList();
            }
        }

        public bool Update<T>(T objectValue) where T : BaseTable
        {
            using (var db = new LiteDatabase(DbFilename))
            {
                return db.GetCollection<T>().Update(objectValue);
            }
        }

        public void SanityCheck()
        {
            try
            {
                using var db = new LiteDatabase(DbFilename);

                db.CollectionExists(nameof(Settings));
            }
            catch (Exception ex)
            {
                // LOG Error

                File.Delete(DbFilename);
            }
        }
    }
}