using System.Linq;
using DMTP.Worker.Common;

namespace DMTP.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverUrl = Constants.DEFAULT_SERVER_URL;

            if (args.Any())
            {
                serverUrl = args[0];
            }

            new Worker(serverUrl).RunAsync();
        }
    }
}