namespace SmartSelectors
{
    internal class ModelResponse : IModelResponse
    {
        public bool Prediction { get; set; }
        public string Label { get; set; }
        public float Accuracy { get; set; }
        public bool IsCached { get; set; }
    }
}
