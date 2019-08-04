using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using DMTP.lib.Common;
using DMTP.lib.ML.Base;

using NLog;

namespace DMTP.REST.Helpers
{
    public static class AssemblyReader
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static List<BasePrediction> LoadAssemblies()
        {
            var files = Directory.GetFiles(AppContext.BaseDirectory, $"*.{Constants.ASSEMBLY_EXTENSION}");

            Log.Debug($"{files.Length} files found to parse");

            var assemblies = new List<BasePrediction>();

            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);

                    assemblies.AddRange(assembly.GetTypes().Where(a => a == typeof(BasePrediction))
                        .Select(a => (BasePrediction) Activator.CreateInstance(a)).ToList());
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to parse {file} due to {ex}");
                }
            }

            Log.Debug($"{assemblies.Count} assemblies loaded from {files.Length} files");

            return assemblies;
        }
    }
}