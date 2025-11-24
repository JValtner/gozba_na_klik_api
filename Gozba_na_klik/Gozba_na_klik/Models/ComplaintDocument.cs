using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gozba_na_klik.Models
{
    public class ComplaintDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("orderId")]
        public int OrderId { get; set; }

        [BsonElement("userId")]
        public int UserId { get; set; }

        [BsonElement("restaurantId")]
        public int RestaurantId { get; set; }

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}

