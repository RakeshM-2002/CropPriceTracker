using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Crops_Price_Tracker.Models.Alerts
{

    public class Alerts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Crop { get; set; }

        public string Market { get; set; }

        public decimal TargetPrice { get; set; }

        public string AboveBelow { get; set; } // "above" | "below"

        public DateTime CreatedAt { get; set; }
    }
}
