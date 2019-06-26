using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DMTP.lib.ML.Base.Objects;
using DMTP.lib.Options;

using Microsoft.ML;

namespace DMTP.lib.ML.Base
{
    public abstract class BasePrediction
    {
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
        }

        protected string OutputModelPath =>
            Path.Combine(AppContext.BaseDirectory, MODEL_NAME);

        protected string SaveModel(ITransformer trainedModel, DataViewSchema schema, TrainerCommandLineOptions options)
        {
            using (var fileStream = new FileStream(OutputModelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                MlContext.Model.Save(trainedModel, schema, fileStream);
            }

            return OutputModelPath;
        }

        public abstract (T Data, string Output) FeatureExtraction<T>(ClassifierResponseItem response)
            where T : BaseData;

        public abstract (string OutputFile, string Metrics) TrainModel(TrainerCommandLineOptions options);
    }
}