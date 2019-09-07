using DMTP.lib.ML.Options.Base;

namespace DMTP.lib.ML.Options
{
    public class ClassifierCommandLineOptions : BaseCommandLineOptions
    {
        public string FileName { get; set; }

        public override string ToString() => $"FileName: {FileName} | Log Level: {LogLevel}";
    }
}