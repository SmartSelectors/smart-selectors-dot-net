namespace SmartSelectors
{
    public interface IModel
    {
        /// <summary>
        /// Predicts the probability of image represented by <paramref name="byteArray"/> being a certain <paramref name="label"/>
        /// </summary>
        /// <returns>
        /// <see cref="T:SmartSelectors.IModelResponse" /> (Prediction, Label, Accuracy, IsCached)
        /// </returns>
        /// <param name="byteArray">Byte array representation of the image to predict</param>
        /// <param name="label">String representing the label of the Icon to find</param>
        IModelResponse Predict(byte[] byteArray, string label);
    }
}