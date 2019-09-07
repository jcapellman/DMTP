using System;

using DMTP.lib.Example.Objects;
using DMTP.lib.ML;
using DMTP.lib.ML.Base;
using DMTP.lib.ML.Options;

using Microsoft.ML;
using Microsoft.ML.Data;

using Newtonsoft.Json;

namespace DMTP.lib.Example
{
    public class ExamplePredictor : BasePrediction
    {
        public override string MODEL_NAME => "Example Predictor";

        private const int STRING_BYTE_MINIMUM = 65526 * 2;

        public override (string json, string Output) FeatureExtraction(ClassifierResponseItem response)
        {
            var clusterData = new ClusterData
            {
                StartStringData = string.Empty,
                EndStringData = string.Empty
            };

            clusterData.StartStringData = GetStrings(response.Data, 0, STRING_BYTE_MINIMUM);
            clusterData.EndStringData = GetStrings(response.Data, response.Data.Length - STRING_BYTE_MINIMUM, STRING_BYTE_MINIMUM);

            return (JsonConvert.SerializeObject(clusterData), $"{(int)response.FileGroup},{clusterData.StartStringData},{clusterData.EndStringData}");
        }

        public override (string OutputFile, string Metrics) TrainModel(TrainerCommandLineOptions options)
        {
            var fileName = FeatureExtractFolder(options);

            var startDate = DateTime.Now;

            var fullData = MlContext.Data.LoadFromTextFile(path: fileName,
                new[]
                {
                    new TextLoader.Column("Label", DataKind.Single, 0),
                    new TextLoader.Column(nameof(ClusterData.StartStringData), DataKind.String, 1),
                    new TextLoader.Column(nameof(ClusterData.EndStringData), DataKind.String, 2)
                },
                hasHeader: false,
                separatorChar: ',');

            var trainTestData = MlContext.Data.TrainTestSplit(fullData, testFraction: 0.2);
            var trainingDataView = trainTestData.TrainSet;
            var testingDataView = trainTestData.TestSet;

            var featuresColumnName = "Features";

            var pipeline = MlContext.Transforms.Text.NormalizeText(nameof(ClusterData.StartStringData))
                .Append(MlContext.Transforms.Text.TokenizeIntoWords(nameof(ClusterData.StartStringData)))
                .Append(MlContext.Transforms.Text.RemoveDefaultStopWords(nameof(ClusterData.StartStringData)))
                .Append(MlContext.Transforms.Conversion.MapValueToKey(nameof(ClusterData.StartStringData)))
                .Append(MlContext.Transforms.Text.ProduceNgrams(nameof(ClusterData.StartStringData)))
                .Append(MlContext.Transforms.NormalizeLpNorm(nameof(ClusterData.StartStringData)))
                .Append(MlContext.Transforms.Text.NormalizeText(nameof(ClusterData.EndStringData))
                .Append(MlContext.Transforms.Text.TokenizeIntoWords(nameof(ClusterData.EndStringData)))
                .Append(MlContext.Transforms.Text.RemoveDefaultStopWords(nameof(ClusterData.EndStringData)))
                .Append(MlContext.Transforms.Conversion.MapValueToKey(nameof(ClusterData.EndStringData)))
                .Append(MlContext.Transforms.Text.ProduceNgrams(nameof(ClusterData.EndStringData)))
                .Append(MlContext.Transforms.NormalizeLpNorm(nameof(ClusterData.EndStringData)))
                .Append(MlContext.Transforms.Concatenate(featuresColumnName, nameof(ClusterData.StartStringData), nameof(ClusterData.EndStringData))));

            var trainer = MlContext.Clustering.Trainers.KMeans(featureColumnName: featuresColumnName, numberOfClusters: 5);

            var trainingPipeline = pipeline.Append(trainer);
            var trainedModel = trainingPipeline.Fit(trainingDataView);

            Log.Debug($"Model trained in {DateTime.Now.Subtract(startDate).TotalSeconds} seconds", options);

            var predictions = trainedModel.Transform(testingDataView);

            var metrics = MlContext.Clustering.Evaluate(predictions, scoreColumnName: "Score", featureColumnName: featuresColumnName);

            Log.Debug($"Average Distance: {metrics.AverageDistance} | Davides Bouldin Index: {metrics.DaviesBouldinIndex}", options);

            return (SaveModel(trainedModel, trainingDataView.Schema, options), $"Average Distance: {metrics.AverageDistance} | Davides Bouldin Index: {metrics.DaviesBouldinIndex}");
        }
    }
}
