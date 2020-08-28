namespace SmartSelectors.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public static class Utils
    {
        public const string PassedPredictionsFolder = "PassedPredictions";
        public const string FailedPredictionsFolder = "FailedPrediction";

        internal static SummaryModel PredictTestSet(IModel model)
        {
            var directoryInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent;
            var folders = Directory.EnumerateDirectories(Path.Combine(directoryInfo.FullName, "Integration/TestSet")).ToList();

            var basePath = $"C:\\Screenshots\\WebIconsPredictor_TestSet_{DateTime.Now:yyyyMMddHHmmss}";
            var summaries = new List<ISummaryModel>();
            var categoriesCount = 0;

            foreach (var folder in folders)
            {
                var label = folder.Split(Path.DirectorySeparatorChar).Last();
                Directory.CreateDirectory($"{basePath}\\{label}\\{Utils.PassedPredictionsFolder}");
                Directory.CreateDirectory($"{basePath}\\{label}\\{Utils.FailedPredictionsFolder}");
                Directory.CreateDirectory($"{basePath}\\z_Failed");

                var directory = new DirectoryInfo(folder);
                var images = directory.GetFiles().Select(x => Image.FromFile(x.FullName));

                var categorySummary = Utils.PredictCategory(images, model, label, basePath);
                var categoryJsonSummary = JsonConvert.SerializeObject(categorySummary);
                File.WriteAllText($"{basePath}\\{label}\\{label}_summary.json", categoryJsonSummary);
                summaries.Add(categorySummary);
                categoriesCount += 1;
            }

            var summary = CreateSummary(summaries, categoriesCount, basePath);
            return summary;
        }

        private static SummaryModel PredictCategory(IEnumerable<Image> images, IModel model, string category, string basePath)
        {
            var summary = new SummaryModel();
            var timeSpans = new List<TimeSpan>();
            var fileName = $"{basePath}\\{{0}}\\{{1}}\\{{2}} - {{3}}.png";

            foreach (var image in images)
            {
                byte[] byteArray;
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    byteArray = ms.ToArray();
                }

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var response = model.Predict(byteArray, category);
                var isExpectedLabel = response.Prediction;
                stopWatch.Stop();

                timeSpans.Add(new TimeSpan(stopWatch.ElapsedTicks));
                if (response.IsCached) summary.CachedImages += 1;
                var label = response.IsCached ? response.Label + "_Cached" : response.Label;
                if (isExpectedLabel)
                {
                    image.Save(string.Format(fileName, category, PassedPredictionsFolder, label, response.Accuracy), ImageFormat.Png);
                    summary.PassedPredictions += 1;
                    continue;
                }
                image.Save(string.Format(fileName, category, FailedPredictionsFolder, label, response.Accuracy), ImageFormat.Png);
                image.Save($"{basePath}\\z_Failed\\Expected {category} - Actual {label} {response.Accuracy}.png", ImageFormat.Png);
                summary.FailedPredictions += 1;
            }

            summary.Label = category;
            summary.TotalPredictions = summary.PassedPredictions + summary.FailedPredictions;
            summary.Accuracy = (float)summary.PassedPredictions / (summary.PassedPredictions + summary.FailedPredictions);
            summary.AverageTime = new TimeSpan(timeSpans.Sum(x => x.Ticks) / summary.TotalPredictions);
            return summary;
        }

        private static SummaryModel CreateSummary(List<ISummaryModel> summaries, int categoriesCount, string basePath)
        {
            var passedPredictions = summaries.Sum(x => x.PassedPredictions);
            var failedPredictions = summaries.Sum(x => x.FailedPredictions);

            var summary = new SummaryModel
            {
                Label = "All",
                TotalPredictions = passedPredictions + failedPredictions,
                PassedPredictions = passedPredictions,
                FailedPredictions = failedPredictions,
                Accuracy = (float)passedPredictions / (passedPredictions + failedPredictions),
                AverageTime = new TimeSpan(summaries.Sum(x => x.AverageTime.Ticks) / categoriesCount),
                CachedImages = summaries.Sum(x => x.CachedImages)
            };
            var jsonSummary = JsonConvert.SerializeObject(summary);
            File.WriteAllText($"{basePath}\\summary.json", jsonSummary);
            return summary;
        }
    }
}
