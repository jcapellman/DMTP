using System;
using System.Collections.Generic;
using System.Linq;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Extensions;
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

        public bool UploadAssembly(byte[] assemblyBytes, string fileName)
        {
            try
            {
                if (assemblyBytes == null)
                {
                    throw new ArgumentNullException(nameof(assemblyBytes));
                }

                var assembly = assemblyBytes.ToAssembly();

                if (assembly == null)
                {
                    throw new ArgumentException("File uploaded is not a valid DLL");
                }

                var predictorTypes =
                    assembly.DefinedTypes.Where(a => a.BaseType == typeof(BasePrediction) && !a.IsAbstract).ToList();

                if (!predictorTypes.Any())
                {
                    throw new ArgumentException("File was a valid DLL, but was not compiled properly");
                }

                var item = new Assemblies
                {
                    FileName = fileName,
                    Predictors = predictorTypes.Select(a => (BasePrediction)Activator.CreateInstance(a)).Select(b => b.MODEL_NAME).ToList(),
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