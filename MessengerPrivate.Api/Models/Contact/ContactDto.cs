using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MessengerPrivate.Api.Models.Contact
{
    public class ContactDto
    {

        public string Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ContactId { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
