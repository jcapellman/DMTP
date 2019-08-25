using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.Managers.Base;
using DMTP.lib.ML.Base;

namespace DMTP.lib.Managers
{
    public class AssemblyManager : BaseManager
    {
        public AssemblyManager(IDatabase database) : base(database)
        {
        }

        public List<string> GetUploadedAssembliesList()
        {
            try
            {
                return _database.GetAll<Assemblies>().Select(a => a.Name).ToList();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get uploaded assemblies list due to {ex}");

                return null;
            }
        }

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