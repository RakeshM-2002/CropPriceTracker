using Crops_Price_Tracker.Infrastructure;
using Crops_Price_Tracker.Models.Crop;
using Crops_Price_Tracker.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Crops_Price_Tracker.Services
{
    public class CropService
    {
        private readonly IMongoCollection<CropPrice> _crop;
        //private readonly DateRangeSettings _range;

        public CropService(
            MongoDBService mongo,
            IOptions<MongoSettings> settings)
        {
            _crop = mongo.Database.GetCollection<CropPrice>(settings.Value.CropCollection);
            //_range = range.Value;
        }

        // Get historical prices for a crop
        public async Task<List<CropHistoryDto>> History(
    string cropName,
    DateTime startDate,
    DateTime endDate)
        {
            // Normalize AND force UTC
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).AddTicks(-1);


            var filter = Builders<CropPrice>.Filter.And(
                Builders<CropPrice>.Filter.Eq(x => x.commodity, cropName),
                Builders<CropPrice>.Filter.Gte(x => x.createdAt, startDate),
                Builders<CropPrice>.Filter.Lte(x => x.createdAt, endDate)
            );

            var rawData = await _crop
                .Find(filter)
                .SortByDescending(x => x.createdAt)
                .Limit(50)
                .Project(x => new
                {
                    x.commodity,
                    x.market,
                    modal_Price = (object)x.modal_Price,
                    min_price = (object)x.min_price,
                    max_price = (object)x.max_price,
                    x.createdAt
                })
                .ToListAsync();

            return rawData.Select(x =>
            {
                decimal.TryParse(x.modal_Price?.ToString(), out var modal);
                decimal.TryParse(x.min_price?.ToString(), out var min);
                decimal.TryParse(x.max_price?.ToString(), out var max);

                return new CropHistoryDto
                {
                    Commodity = x.commodity,
                    Market = x.market,
                    ModalPrice = modal,
                    MinPrice = min,
                    MaxPrice = max,
                    CreatedAt = x.createdAt
                };
            }).ToList();
        }


        // Analyze price trends for a specific crop and market
        public async Task<PriceAnalysisResult> Analyze(
     string cropName,
     string marketName,
     DateTime startDate,
     DateTime endDate)
        {
            // Normalize dates (THIS IS THE KEY)
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).AddTicks(-1);


            var filter = Builders<CropPrice>.Filter.And(
                Builders<CropPrice>.Filter.Eq(x => x.commodity, cropName),
                Builders<CropPrice>.Filter.Eq(x => x.market, marketName),
                Builders<CropPrice>.Filter.Gte(x => x.createdAt, startDate),
                Builders<CropPrice>.Filter.Lte(x => x.createdAt, endDate)
            );

            var data = await _crop
                .Find(filter)
                .SortBy(x => x.createdAt)
                .ToListAsync();

            if (data.Count < 2)
                return new PriceAnalysisResult { HasSufficientData = false };

            var history = data
                .Select(x => new CropAnalysisHistoryDto
                {
                    Date = x.createdAt,
                    PricePerKg = x.modal_Price
                })
                .Where(x => x.PricePerKg > 0)
                .ToList();

            var prices = history.Select(x => x.PricePerKg).ToList();

            decimal latest = prices.Last();
            decimal previous = prices[^2];

            return new PriceAnalysisResult
            {
                HasSufficientData = true,
                LatestPrice = latest,
                PreviousPrice = previous,
                DayChange = latest - previous,
                DayChangePercent =  previous == 0 ? 0 : ((latest - previous) / previous) * 100,
                Average7 = prices.TakeLast(Math.Min(7, prices.Count)).Average(),
                Average30 = prices.TakeLast(Math.Min(30, prices.Count)).Average(),
                Minimum = prices.Min(),
                Maximum = prices.Max(),
                StandardDeviation = CalculateStandardDeviation(prices),
                Trend = latest > previous ? "Increasing" : "Decreasing",
                Summary = $"{cropName} price trend analyzed for {marketName}",
                FullHistory = history
            };
        }


        //public async Task<DateRangeSettings> SetDate(DateTime startDate, DateTime endDate)
        //{
        //    var dateRange = new DateRangeSettings
        //    {
        //        StartDate = startDate,
        //        EndDate = endDate
        //    };

        //    return await Task.FromResult(dateRange);
        //}



        // Get latest prices filtered by crops and markets (for alerts)
        public async Task<List<CropPrice>> GetLatestPrices(List<string> crops, List<string> markets)
        {
            var filter = Builders<CropPrice>.Filter.In(x => x.commodity, crops) &
                         Builders<CropPrice>.Filter.In(x => x.market, markets);

            return await _crop
                .Find(filter)
                .SortByDescending(x => x.createdAt)
                .ToListAsync();
        }

        private decimal CalculateMovingAverage(List<decimal> prices, int days)
        {
            return prices.Count < days
                ? prices.Average()
                : prices.TakeLast(days).Average();
        }

        private decimal CalculateStandardDeviation(List<decimal> prices)
        {
            var avg = prices.Average();
            var variance = prices.Average(v => (v - avg) * (v - avg));
            return (decimal)Math.Sqrt((double)variance);
        }

        private string GetTrendDirection(decimal dayChange, decimal latest)
        {
            if (Math.Abs(dayChange) < 0.1m) return "Stable";
            return dayChange > 0 ? "Increasing" : "Decreasing";
        }

        private string GenerateSummaryText(
            string crop, string market, 
            decimal latest, decimal change, decimal percent)
        {
            string direction = change > 0 ? "up" : change < 0 ? "down" : "stable";
            return $"{crop} price in {market} is ₹{latest}. Trend is {direction} with a daily change of {Math.Round(percent, 2)}%.";
        }
    }
}
