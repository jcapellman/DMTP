using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DMTP.lib.Enums;
using DMTP.lib.ML.Base.Objects;
using DMTP.lib.Options;

using Microsoft.ML;

using NLog;

namespace DMTP.lib.ML.Base
{
    public abstract class BasePrediction
    {
        protected static Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public abstract string MODEL_NAME { get; }

        private const int FILE_ENCODING = 1252;

        private const int BUFFER_SIZE = 2048;

        protected static readonly MLContext MlContext = new MLContext(Common.Constants.ML_SEED);

        private static Regex _stringRex;

        protected BasePrediction()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _stringRex = new Regex(@"[ -~\t]{8,}", RegexOptions.Compiled);
        }

        protected string GetStrings(byte[] data, int start, int length)
        {
            try {
                var stringLines = new StringBuilder();

                if (data == null || data.Length == 0)
                {
                    return stringLines.ToString();
                }

                var dataToParse = data.Length < data.Length - length - start ? data : data.Skip(start).Take(length).ToArray();

                using (var ms = new MemoryStream(dataToParse, false))
                {
                    using (var streamReader = new StreamReader(ms, Encoding.GetEncoding(FILE_ENCODING), false, BUFFER_SIZE, false))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            var line = streamReader.ReadLine();

                            if (string.IsNullOrEmpty(line))
                            {
                                continue;
                            }

                            line = line.Replace("^", "").Replace(")", "").Replace("-", "");

                            stringLines.Append(string.Join(string.Empty,
                                _stringRex.Matches(line).Where(a => !string.IsNullOrEmpty(a.Value) && !string.IsNullOrWhiteSpace(a.Value)).ToList()));
                        }
                    }
                }

                return string.Join(string.Empty, stringLines);
            } catch (Exception ex)
            {
                Log.Error($"Failure in Getting Strings for {MODEL_NAME} due to {ex}");

                return null;
            }
        }

        protected string OutputModelPath =>
            Path.Combine(AppContext.BaseDirectory, MODEL_NAME);

        protected string SaveModel(ITransformer trainedModel, DataViewSchema schema, TrainerCommandLineOptions options)
        {
            try
            {
                using (var fileStream = new FileStream(OutputModelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    MlContext.Model.Save(trainedModel, schema, fileStream);
                }

                return OutputModelPath;
            }
            catch (Exception ex)
            {
                Log.Error($"Failure in saving model to {OutputModelPath} due to {ex}");

                return null;
            }
        }

        protected string FeatureExtractFolder(TrainerCommandLineOptions options)
        {
            var fileName = Path.Combine(AppContext.BaseDirectory, $"{DateTime.Now.Ticks}.txt");

            var files = Directory.GetFiles(options.FolderOfData);

            Log.Debug($"{files.Length} Files found for training...", options);

            var stopWatch = DateTime.Now;

            var extractions = new ConcurrentQueue<string>();

            var classifications = new ConcurrentQueue<FileGroupType>();

            Parallel.ForEach(files, file =>
            {
                var response = new ClassifierResponseItem(File.ReadAllBytes(file), file, true);

                var (data, output) = FeatureExtraction(response);

                classifications.Enqueue(response.FileGroup);

                extractions.Enqueue(output);
            });

            File.WriteAllText(fileName, string.Join(System.Environment.NewLine, extractions));

            var featureBreakdown = (from classification in classifications.GroupBy(a => a).Select(a => a.Key)
                let count = classifications.Count(a => a == classification)
                let percentage = Math.Round((double)count / files.Length * 100.0, 0)
                select $"{classification}: {(double)count} ({percentage}%)").ToList();

            Log.Debug(string.Join("|", featureBreakdown), options);

            Log.Debug($"Feature Extraction took {DateTime.Now.Subtract(stopWatch).TotalSeconds} seconds", options);

            return fileName;
        }

        public abstract (string json, string Output) FeatureExtraction(ClassifierResponseItem response);

        public abstract (string OutputFile, string Metrics) TrainModel(TrainerCommandLineOptions options);
    }
}