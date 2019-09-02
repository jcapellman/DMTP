using System.Collections.Generic;

using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Databases.Tables
{
    public class Assemblies : BaseTable
    {
        public string FileName { get; set; }

        public List<string> Predictors { get; set; }

        public byte[] Data { get; set; }
    }
}