using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Manager
{
    public class DatabaseManager
    {
        private readonly IDatabase _database;
        private readonly IDatabase _cache;

        public DatabaseManager(IDatabase database, IDatabase cache)
        {
            _database = database;
            _cache = cache;
        }

        public Guid Insert<T>(T objectValue) where T : BaseTable
        {
            var result = _database.Insert(objectValue);
    
            _cache?.DeleteAll<T>();

            return result;
        }

        public bool DeleteAll<T>() where T : BaseTable
        {
            _cache?.DeleteAll<T>();

            return _database.DeleteAll<T>();
        }

        public bool Delete<T>(Guid id) where T : BaseTable
        {
            var result = _database.Delete<T>(id);

            if (result)
            {
                _cache?.DeleteAll<T>();
            }

            return result;
        }

        public T GetOne<T>(Expression<Func<T, bool>> query) where T : BaseTable => _database.GetOne(query);

        public T GetOneById<T>(Guid id) where T : BaseTable => _database.GetOneById<T>(id);

        public List<T> GetAll<T>() where T : BaseTable
        {
            var cacheResult = _cache?.GetAll<T>();

            if (cacheResult != null)
            {
                return cacheResult;
            }

            var dbResult = _database.GetAll<T>();

            _cache?.InsertRange(dbResult);

            return dbResult;
        }

        public bool Update<T>(T objectValue) where T : BaseTable
        {
            var result = _database.Update(objectValue);

            if (!result)
            {
                return false;
            }

            _cache?.DeleteAll<T>();

            return true;
        }
    }
}