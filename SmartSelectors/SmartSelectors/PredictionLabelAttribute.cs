namespace SmartSelectors
{
    using System;

    public class PredictionLabelAttribute : Attribute
    {
        public PredictionLabelAttribute(string label)
        {
            Label = label;
        }

        public string Label { get; }
    }
}
