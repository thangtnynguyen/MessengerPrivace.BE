namespace MessengerPrivate.Api.Models.Conversation
{
    public class DeleteParticipantsConversationRequest
    {

        public string Id { get; set; }
        public List<Guid> Participants { get; set; } // List of User Ids


    }
}
