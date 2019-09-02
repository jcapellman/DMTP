using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers.Base;
using DMTP.lib.ML.Base;

namespace DMTP.lib.Managers
{
    public class AssemblyManager : BaseManager
    {
        public AssemblyManager(DatabaseManager database) : base(database)
        {
        }

        public List<Assemblies> GetUploadedAssembliesList()
        {
            try
            {
                return _database.GetAll<Assemblies>();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get uploaded assemblies list due to {ex}");

                return null;
            }
        }

        public bool DeleteAssembly(Guid id) => _database.Delete<Assemblies>(id);

        public bool UploadAssembly(byte[] assemblyBytes)
        {
            try
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

                _database.Insert(item);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failure to upload assembly {ex.StackTrace}");

                return false;
            }
        }
    }
}