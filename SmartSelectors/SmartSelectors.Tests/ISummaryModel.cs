namespace SmartSelectors.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface ISummaryModel
    {
        string Label { get; set; }
        int TotalPredictions { get; set; }
        int PassedPredictions { get; set; }
        int FailedPredictions { get; set; }
        float Accuracy { get; set; }
        TimeSpan AverageTime { get; set; }
        int CachedImages { get; set; }
    }
}