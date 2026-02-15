using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crops_Price_Tracker.Models.Crop
{
    [BsonIgnoreExtraElements]
    public class CropPrice
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string commodity { get; set; }
        public string market { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string max_price { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string min_price { get; set; }

        [BsonElement("modal_Price")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal modal_Price { get; set; }

        public string state { get; set; }
        public DateTime createdAt { get; set; }

    }
}
