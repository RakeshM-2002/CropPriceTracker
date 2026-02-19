namespace Crops_Price_Tracker.Models.Crop
{
    public class PriceAnalysisResult
    {
        public bool HasSufficientData { get; set; }


        public decimal LatestPrice { get; set; }
        public decimal PreviousPrice { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePercent { get; set; }

        public decimal Average7 { get; set; }
        public decimal Average30 { get; set; }

        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }

        public decimal StandardDeviation { get; set; }

        public string Trend { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;

        public List<CropAnalysisHistoryDto> FullHistory { get; set; } = new();
    }
}
