using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MessengerPrivate.Api.Models.User;
using MessengerPrivate.Api.Models.Messenger;

namespace MessengerPrivate.Api.Models.Conversation
{
    public class ConversationDto
    {

        public string Id { get; set; }

        public string? Type { get; set; } // personal or group

        public List<Guid>? Participants { get; set; } // List of User Ids

        public List<UserDto>? ParticipantInfos { get; set; } // List of User Infos

        public List<Guid>? Administrators { get; set; } // List of Administrator User Ids

        public string? Name { get; set; }

        public string? AvatarUrl { get; set; }


        //public string? LastMessageId { get; set; }

        //public string? LastMessageContent { get; set; } 

        public MessengerDto? Messenger { get; set; }

    }
}
