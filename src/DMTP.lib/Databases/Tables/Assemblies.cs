using DMTP.lib.Databases.Tables.Base;

namespace DMTP.lib.Databases.Tables
{
    public class Assemblies : BaseTable
    {
        public string Name { get; set; }

        public byte[] Data { get; set; }
    }
}