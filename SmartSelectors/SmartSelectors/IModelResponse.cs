namespace SmartSelectors
{
    public interface IModelResponse
    {
        bool Prediction { get; set; }
        string Label { get; set; }
        float Accuracy { get; set; }
        bool IsCached { get; set; }
    }
}