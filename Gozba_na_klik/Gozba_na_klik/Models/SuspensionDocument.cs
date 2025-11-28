using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gozba_na_klik.Models
{
    public class SuspensionDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("restaurantId")]
        public int RestaurantId { get; set; }

        [BsonElement("suspensionReason")]
        public string SuspensionReason { get; set; } = string.Empty;

        [BsonElement("suspendedAt")]
        public DateTime SuspendedAt { get; set; }

        [BsonElement("suspendedBy")]
        public int SuspendedBy { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "SUSPENDED";

        [BsonElement("appealText")]
        public string? AppealText { get; set; }

        [BsonElement("appealDate")]
        public DateTime? AppealDate { get; set; }

        [BsonElement("decisionDate")]
        public DateTime? DecisionDate { get; set; }

        [BsonElement("decisionBy")]
        public int? DecisionBy { get; set; }
    }
}

