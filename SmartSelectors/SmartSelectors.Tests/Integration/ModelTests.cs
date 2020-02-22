namespace SmartSelectors.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class ModelTests
    {
        [Test]
        public void IconPredictionWithOnnxModel()
        {
            var summary = PredictTestSet(new OnnxModel(null));
            summary.AverageTime.Should().BeLessOrEqualTo(TimeSpan.FromMilliseconds(150));
        }

        [Test]
        public void IconPredictionWithApiModel()
        {
            var summary = PredictTestSet(new ApiModel());
            summary.AverageTime.Should().BeLessOrEqualTo(TimeSpan.FromMilliseconds(500));
        }

        private static SummaryModel PredictTestSet(IModel model)
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

            var summary = Utils.CreateSummary(summaries, categoriesCount, basePath);
            summary.Accuracy.Should().BeGreaterOrEqualTo((float) 0.9);
            return summary;
        }
    }
}
