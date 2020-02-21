namespace SmartSelectors
{
    /// <summary>
    /// Response to the model prediction
    /// </summary>
    public interface IModelResponse
    {
        /// <summary>
        /// Bool indicating whether the image matches the label
        /// </summary>
        bool Prediction { get; set; }

        /// <summary>
        /// Label predicted
        /// </summary>
        string Label { get; set; }
        
        /// <summary>
        /// Accuracy of the prediction for the Label in the response
        /// </summary>
        float Accuracy { get; set; }
        
        /// <summary>
        /// Bool indicating whether the image was already cached
        /// </summary>
        bool IsCached { get; set; }
    }
}