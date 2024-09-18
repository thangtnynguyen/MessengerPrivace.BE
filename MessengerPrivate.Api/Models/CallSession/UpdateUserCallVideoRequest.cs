namespace MessengerPrivate.Api.Models.CallSession
{
    public class UpdateUserCallVideoRequest
    {
        public string ConversationId { get; set; }

        public string CallSessionId { get; set; } 

        public Guid UserId { get; set; }

        public string Type { get; set; }// mutecamera, mutemicro

        public bool? MuteCamera { get; set; }

        public bool? MuteMicro { get; set; } 

        public string? Status { get; set; } 

    }
}
