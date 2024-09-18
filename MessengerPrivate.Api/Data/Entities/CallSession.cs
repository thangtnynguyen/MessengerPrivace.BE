using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MessengerPrivate.Api.Data.Entities.Interface;

namespace MessengerPrivate.Api.Data.Entities
{
    public class CallSession: EntityBase
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ConversationId")]
        [BsonRepresentation(BsonType.String)]
        public string ConversationId { get; set; }

        [BsonElement("CallerId")]
        [BsonRepresentation(BsonType.String)]
        public Guid CallerId { get; set; }

        [BsonElement("UserCallVideos")]
        public List<UserCallVideo> UserCallVideos { get; set; } // List of User Join Video Call

        [BsonElement("StartTime")]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [BsonElement("EndTime")]
        public DateTime EndTime { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; } // ongoing, ended, missed
    }

    public class UserCallVideo
    {
        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        [BsonElement("UserName")]
        public string UserName { get; set; }

        [BsonElement("UserAvatarUrl")]
        public string? UserAvatarUrl { get; set; }

        [BsonElement("MuteCamera")]
        public bool? MuteCamera { get; set; }

        [BsonElement("MuteMicro")]
        public bool? MuteMicro { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; } // online, offline, done join


    }
}
