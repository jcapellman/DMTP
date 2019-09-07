using DMTP.lib.ML.Options.Base;

namespace DMTP.lib.ML.Options
{
    public class TrainerCommandLineOptions : BaseCommandLineOptions
    {
        public string FolderOfData { get; set; }

        public string ModelType { get; set; }

        public override string ToString() => $"Folder Containing Data: {FolderOfData} | Log Level: {LogLevel}";
    }
}