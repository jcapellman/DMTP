using System;

namespace DMTP.lib.Databases.Tables
{
    public class Assemblies
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }
    }
}