using Crops_Price_Tracker.Infrastructure;
using Crops_Price_Tracker.Models.Alerts;
using Crops_Price_Tracker.Models.Crop;
using Crops_Price_Tracker.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Crops_Price_Tracker.Services
{
    public class AlertServices
    {
        private readonly IMongoCollection<Alerts> _alerts;
        private readonly IMongoCollection<CropPrice> _crop;

        public AlertServices(MongoDBService mongo, IOptions<MongoSettings> settings)
        {
            _alerts = mongo.Database.GetCollection<Alerts>(
                settings.Value.AlertCollection
            );
            _crop = mongo.Database.GetCollection<CropPrice>(
                settings.Value.CropCollection
            );
        }

        /* ========= ADD ALERT ========= */
        public async Task AddAlert(Alerts alert)
        {
            alert.CreatedAt = DateTime.UtcNow;
            await _alerts.InsertOneAsync(alert);
        }

        /* ========= GET ALERTS BY USER ========= */
        public async Task<List<AlertResponseDto>> GetAlertsByUser(
        string userId,
        DateTime startDate,
        DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).AddTicks(-1);

            var alerts = await _alerts
                .Find(a => a.UserId == userId)
                .ToListAsync();

            var response = new List<AlertResponseDto>();

            foreach (var alert in alerts)
            {
                var latestCrop = await _crop
                    .Find(x =>
                        x.commodity == alert.Crop &&
                        x.market == alert.Market &&
                        x.createdAt >= startDate &&
                        x.createdAt <= endDate)
                    .SortByDescending(x => x.createdAt)
                    .ThenByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                if (latestCrop == null)
                    continue;

                var latestPrice = latestCrop.modal_Price;

                bool isTriggered =
                    (alert.AboveBelow == "ABOVE" && latestPrice > alert.TargetPrice) ||
                    (alert.AboveBelow == "BELOW" && latestPrice < alert.TargetPrice);

                if (!isTriggered)
                    continue;

                response.Add(new AlertResponseDto
                {
                    Crop = alert.Crop,
                    Market = alert.Market,
                    LatestPrice = latestPrice,
                    TargetPrice = alert.TargetPrice,
                    Condition = alert.AboveBelow,
                    CreatedAt = alert.CreatedAt,
                    Message = $"Current Price for {alert.Crop} in {alert.Market} market is {(alert.AboveBelow == "ABOVE" ? "exceeded" : "dropped below")} the target price!"
                });
            }

            return response;
        }

    }
}
