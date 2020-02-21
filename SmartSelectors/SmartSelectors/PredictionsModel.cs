namespace SmartSelectors
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class PredictionsModel
    {
        public string ErrorMessage { get; set; }
        public string GetImageFileTime { get; set; }
        public string ImageToTensorTime { get; set; }
        public string InferenceTime { get; set; }
        public List<PredictionModel> Result { get; set; }
        public bool Success { get; set; }
    }
}
