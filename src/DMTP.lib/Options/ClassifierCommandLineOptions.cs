using DMTP.lib.Options.Base;

namespace DMTP.lib.Options
{
    public class ClassifierCommandLineOptions : BaseCommandLineOptions
    {
        public string FileName { get; set; }

        public override string ToString() => $"FileName: {FileName} | Log Level: {LogLevel}";
    }
}