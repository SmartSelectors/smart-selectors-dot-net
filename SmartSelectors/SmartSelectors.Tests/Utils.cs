namespace SmartSelectors.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public static class Utils
    {
        public const string PassedPredictionsFolder = "PassedPredictions";
        public const string FailedPredictionsFolder = "FailedPrediction";

        public static SummaryModel PredictCategory(IEnumerable<Image> images, IModel model, string category, string basePath)
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

        public static SummaryModel CreateSummary(List<ISummaryModel> summaries, int categoriesCount, string basePath)
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
