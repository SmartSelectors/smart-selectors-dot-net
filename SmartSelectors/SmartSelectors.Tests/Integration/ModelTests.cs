namespace SmartSelectors.Tests.Integration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using NUnit.Framework;

    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class ModelTests
    {
        [Test]
        public void IconPredictionWithOnnxModel()
        {
            var summary = Utils.PredictTestSet(new OnnxModel(null));
            summary.Accuracy.Should().BeGreaterOrEqualTo((float)0.9);
            summary.AverageTime.Should().BeLessOrEqualTo(TimeSpan.FromMilliseconds(150));
        }

        [Test]
        public void IconPredictionWithApiModel()
        {
            var summary = Utils.PredictTestSet(new ApiModel());
            summary.Accuracy.Should().BeGreaterOrEqualTo((float)0.9);
            summary.AverageTime.Should().BeLessOrEqualTo(TimeSpan.FromMilliseconds(500));
        }
    }
}
