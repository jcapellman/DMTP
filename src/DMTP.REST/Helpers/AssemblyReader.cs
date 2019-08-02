using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using DMTP.lib.ML.Base;

namespace DMTP.REST.Helpers
{
    public static class AssemblyReader
    {
        public static List<BasePrediction> LoadAssemblies()
        {
            var files = Directory.GetFiles(AppContext.BaseDirectory, "*.dasm");

            var assemblies = new List<BasePrediction>();

            foreach (var file in files)
            {
                var assembly = Assembly.LoadFrom(file);

                if (assembly == null)
                {
                    // LOG
                    continue;
                }

                assemblies.AddRange(assembly.GetTypes().Where(a => a == typeof(BasePrediction)).Select(a => (BasePrediction)Activator.CreateInstance(a)).ToList());
            }

            return assemblies;
        }
    }
}