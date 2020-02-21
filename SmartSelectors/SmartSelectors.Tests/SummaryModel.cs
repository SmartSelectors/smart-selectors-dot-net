namespace SmartSelectors.Tests
{
    using System;

    public class SummaryModel : ISummaryModel
    {
        public string Label { get; set; }
        public int TotalPredictions { get; set; } = 0;
        public int PassedPredictions { get; set; } = 0;
        public int FailedPredictions { get; set; } = 0;
        public float Accuracy { get; set; } = 0;
        public TimeSpan AverageTime { get; set; }
        public int CachedImages { get; set; } = 0;
    }
}
