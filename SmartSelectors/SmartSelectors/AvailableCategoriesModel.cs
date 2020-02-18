namespace SmartSelectors
{
    using System.Collections.Generic;

    public class AvailableCategoriesModel
    {
        public string ErrorMessage { get; set; }
        public List<ModelSummary> Result { get; set; }
        public bool Success { get; set; }
    }
}
