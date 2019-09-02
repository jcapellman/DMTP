using DMTP.lib.ML.Base.Objects;

using Microsoft.ML.Data;

namespace DMTP.lib.Example.Objects
{
    public class ClusterDataPrediction : BasePredictionData
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;

        [ColumnName("Score")]
        public float[] Distances;
    }
}