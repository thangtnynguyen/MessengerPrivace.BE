using MessengerPrivate.Api.Data.Entities;

namespace MessengerPrivate.Api.Models.CallSession
{
    public class CallSessionDto
    {

        public string Id { get; set; }

        public string ConversationId { get; set; }

        public Guid CallerId { get; set; }

        public List<UserCallVideo> UserCallVideos { get; set; } 

        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime EndTime { get; set; }

        public string Status { get; set; } 
    }
}
