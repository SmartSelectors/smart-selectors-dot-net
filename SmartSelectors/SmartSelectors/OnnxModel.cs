using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace SmartSelectors
{
    public sealed class OnnxModel : IModel, IDisposable
    {
        private const float Threshold = (float)0.5;
        private const string OnnxFileExtension = ".onnx";
        private const string CategoriesFileExtension = ".txt";
        private static readonly string ModelsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models");
        private readonly SimpleMemoryCache<ModelResponse> _imageCache;
        private readonly InferenceSession _inferenceSession;
        private readonly IReadOnlyDictionary<string, NodeMetadata> _inputMetadata;
        private readonly int[] _inputDimensions;
        private readonly int _width;
        private readonly int _height;
        private readonly int _inputSizeFlat;
        private readonly IReadOnlyCollection<string> _categoriesInModel;

        public OnnxModel() : this(null)
        {
        }

        public OnnxModel(string modelName)
        {
            _imageCache = new SimpleMemoryCache<ModelResponse>();

            var modelPath = GetModelPath(modelName);
            var categoriesFilePath = GetCategoriesFilePath(modelName);

            if (modelName == null && Path.GetFileNameWithoutExtension(modelPath) != Path.GetFileNameWithoutExtension(categoriesFilePath))
            {
                throw new Exception("The Model File Name doesn't match the Categories File Name");
            }

            var options = new SessionOptions
            {
                GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_EXTENDED
            };
            _inferenceSession = new InferenceSession(modelPath, options);
            _inputMetadata = _inferenceSession.InputMetadata;
            _inputDimensions = _inputMetadata.ElementAt(0).Value.Dimensions.Select(Math.Abs).ToArray();
            var inputSize = _inputDimensions.Skip(1).ToArray();
            _width = inputSize[0];
            _height = inputSize[1];
            _inputSizeFlat = inputSize.Aggregate((a, b) => a * b);
            _categoriesInModel = File.ReadAllText(categoriesFilePath).Split('\n').Select(x => x.Split(':')[0]).ToArray();
        }

        public IModelResponse Predict(byte[] imageBytes, string label)
        {
            var byteArrayKey = Convert.ToBase64String(imageBytes);
            var cachedResponse = _imageCache.TryGet(byteArrayKey);

            if (cachedResponse != null)
            {
                return new ModelResponse
                {
                    Prediction = cachedResponse.Label.Equals(label) && cachedResponse.Accuracy > Threshold,
                    Label = cachedResponse.Label,
                    Accuracy = cachedResponse.Accuracy,
                    IsCached = true
                };
            }

            var image = PreprocessImage(imageBytes);
            var container = new List<NamedOnnxValue>();

            foreach (var name in _inputMetadata.Keys)
            {
                var tensor = new DenseTensor<float>(image, _inputDimensions);
                container.Add(NamedOnnxValue.CreateFromTensor(name, tensor));
            }

            var inferenceResult = _inferenceSession.Run(container).ElementAt(0).AsTensor<float>().ToList();
            var response = inferenceResult.Zip(_categoriesInModel, (k, v) => new ModelResponse { Label = v, Accuracy = k, Prediction = v == label })
                .OrderByDescending(x => x.Accuracy).First();

            _imageCache.Set(byteArrayKey, response);

            return response;
        }

        private static string GetModelPath(string modelName)
        {
            var modelSearchPattern = (modelName ?? "*") + OnnxFileExtension;
            var modelFiles = Directory.EnumerateFiles(ModelsFolderPath, modelSearchPattern, SearchOption.TopDirectoryOnly).ToList();

            if (!modelFiles.Any())
            {
                throw new InvalidOperationException($"No ONNX models found in Models folder with search pattern: {modelSearchPattern}");
            }

            if (modelFiles.Count > 1)
            {
                throw new InvalidOperationException($"More than one ONNX model found in Models folder with search pattern: {modelSearchPattern}");
            }

            return modelFiles[0];
        }

        private static string GetCategoriesFilePath(string modelName)
        {
            var categoriesSearchPattern = (modelName ?? "*") + CategoriesFileExtension;
            var categoriesFiles = Directory.EnumerateFiles(ModelsFolderPath, categoriesSearchPattern, SearchOption.TopDirectoryOnly).ToList();

            if (!categoriesFiles.Any())
            {
                throw new InvalidOperationException($"No Categories file found in Models folder with search pattern: {categoriesSearchPattern}");
            }

            if (categoriesFiles.Count > 1)
            {
                throw new InvalidOperationException($"More than one Categories file found in Models folder with search pattern: {categoriesSearchPattern}");
            }

            return categoriesFiles[0];
        }

        private float[] PreprocessImage(byte[] imageBytes)
        {
            var imageFile = Image.FromStream(new MemoryStream(imageBytes));
            var destImage = new Bitmap(_width, _height);

            destImage.SetResolution(imageFile.HorizontalResolution, imageFile.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                var destRect = new Rectangle(0, 0, _width, _height);
                graphics.DrawImage(imageFile, destRect, 0, 0, imageFile.Width, imageFile.Height, GraphicsUnit.Pixel,
                    wrapMode);
            }

            var image = new float[_inputSizeFlat];
            var i = 0;

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var color = destImage.GetPixel(y, x);

                    image[i] = (float)color.R / 255;
                    image[i + 1] = (float)color.G / 255;
                    image[i + 2] = (float)color.B / 255;
                    i += 3;
                }
            }

            return image;
        }

        public void Dispose()
        {
            _inferenceSession.Dispose();
        }
    }
}
