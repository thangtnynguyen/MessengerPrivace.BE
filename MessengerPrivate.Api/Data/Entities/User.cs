using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace MessengerPrivate.Api.Data.Entities
{

    public class User: MongoIdentityUser<Guid>
    {

        [BsonElement("ProfilePicture")]
        public string ProfilePicture { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("AvatarUrl")]
        public string? AvatarUrl { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [BsonElement("DeletedAt")]
        public DateTime? DeletedAt { get; set; }



        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }


    }
}
