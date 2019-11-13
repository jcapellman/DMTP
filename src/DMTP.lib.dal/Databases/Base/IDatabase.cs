using System;
using System.Collections.Generic;

using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Databases.Base
{
    public interface IDatabase
    {
        Guid Insert<T>(T objectValue) where T : BaseTable;

        bool InsertRange<T>(List<T> objects) where T : BaseTable;

        bool DeleteAll<T>() where T : BaseTable;

        bool Delete<T>(Guid id) where T : BaseTable;

        T GetOneById<T>(Guid id) where T : BaseTable;

        T GetOne<T>(System.Linq.Expressions.Expression<Func<T, bool>> query) where T : BaseTable;

        List<T> GetAll<T>() where T : BaseTable;

        bool Update<T>(T objectValue) where T : BaseTable;

        void SanityCheck();
    }
}