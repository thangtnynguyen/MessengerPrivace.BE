using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MessengerPrivate.Api.Models.Conversation
{
    public class CreateConversationRequest
    {

        public string Type { get; set; } // personal or group

        public List<Guid> Participants { get; set; } // List of User Ids

        public List<Guid>? Administrators { get; set; } // List of Administrator User Ids

        public string? Name { get; set; }

        public IFormFile? AvatarUrlFile { get; set; }

        //public string? AvatarUrl { get; set; }

        public string? LastMessageId { get; set; }

    }
}
