namespace SmartSelectors
{
    using System;
    using System.Net;
    using RestSharp;

    public sealed class ApiModel : IModel
    {
        private const float Threshold = (float)0.5;
        private const string BaseUrl = "https://webiconsimagepredictor.azurewebsites.net/";
        private readonly RestClient _restClient;
        private readonly SimpleMemoryCache<ModelResponse> _imageCache;

        public ApiModel()
        {
            _imageCache = new SimpleMemoryCache<ModelResponse>();
            _restClient = new RestClient(BaseUrl);
        }

        public IModelResponse Predict(byte[] byteArray, string label)
        {
            var byteArrayKey = Convert.ToBase64String(byteArray);
            var cachedResponse = _imageCache.TryGet(byteArrayKey);
            if (cachedResponse != null)
                return new ModelResponse
                {
                    Prediction = cachedResponse.Label.Equals(label) && cachedResponse.Accuracy > Threshold,
                    Label = cachedResponse.Label,
                    Accuracy = cachedResponse.Accuracy,
                    IsCached = true
                };
            var prediction = GetPrediction(byteArray);
            if (prediction == null) return new ModelResponse
            {
                Prediction = false,
                Label = "NA",
                Accuracy = 0,
                IsCached = false
            };

            var response = new ModelResponse
            {
                Prediction = prediction.Label.Equals(label) && prediction.Prediction > Threshold,
                Label = prediction.Label,
                Accuracy = prediction.Prediction,
                IsCached = false
            };

            if (!prediction.Label.Equals(string.Empty)) _imageCache.Set(byteArrayKey, response);
            return response;
        }

        private PredictionModel GetPrediction(byte[] byteArray)
        {
            var request = new RestRequest("predict/", Method.POST);
            request.AddFileBytes("image", byteArray, "image");
            request.AddHeader("Content-Type", "multipart/form-data");
            var response = _restClient.Execute<PredictionsModel>(request);
            return response.StatusCode.Equals(HttpStatusCode.OK) ? response.Data.Result[0] : null;
        }
    }
}
