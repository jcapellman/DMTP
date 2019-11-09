using System;
using System.Reflection;

namespace DMTP.lib.Extensions
{
    public static class AssemblyLoader
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public static Assembly ToAssembly(this byte[] assemblyBytes)
        {
            try
            {
                return Assembly.Load(assemblyBytes);
            }
            catch (ArgumentNullException)
            {
                Log.Error("ToAssembly: assemblyBytes was null");

                return null;
            }
            catch (BadImageFormatException)
            {
                Log.Error("ToAssembly: Failed to load bytes");

                return null;
            }
        }
    }
}