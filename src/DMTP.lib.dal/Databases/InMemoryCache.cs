using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Databases
{
    public class InMemoryCache : IDatabase
    {
        private readonly Dictionary<string, object> _cache;

        public InMemoryCache()
        {
            _cache = new Dictionary<string, object>();
        }

        public Guid Insert<T>(T objectValue) where T : BaseTable
        {
            return Guid.Empty;
        }

        public bool InsertRange<T>(List<T> objects) where T : BaseTable
        {
            _cache[nameof(T)] = objects;

            return true;
        }

        public bool DeleteAll<T>() where T : BaseTable
        {
            _cache.Remove(nameof(T));

            return true;
        }

        public bool Delete<T>(Guid id) where T : BaseTable
        {
            return false;
        }

        public T GetOneById<T>(Guid id) where T : BaseTable
        {
            return null;
        }

        public T GetOne<T>(Expression<Func<T, bool>> query) where T : BaseTable
        {
            return null;
        }

        public List<T> GetAll<T>() where T : BaseTable
        {
            if (_cache.ContainsKey(nameof(T)))
            {
                return _cache[nameof(T)] as List<T>;
            }

            return null;
        }

        public bool Update<T>(T objectValue) where T : BaseTable
        {
            throw new NotImplementedException();
        }

        public void SanityCheck()
        {
            // Nothing to do here
        }
    }
}
