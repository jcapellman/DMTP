using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Databases.Tables
{
    public class Assemblies : BaseTable
    {
        public string Name { get; set; }

        public byte[] Data { get; set; }
    }
}