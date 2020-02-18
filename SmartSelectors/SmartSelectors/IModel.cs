namespace SmartSelectors
{
    public interface IModel
    {
        IModelResponse Predict(byte[] byteArray, string label);
    }
}