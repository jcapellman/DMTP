using System;

using DMTP.lib.Databases.Tables.Base;

namespace DMTP.lib.Databases.Tables
{
    public class Assemblies : BaseTable
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }
    }
}