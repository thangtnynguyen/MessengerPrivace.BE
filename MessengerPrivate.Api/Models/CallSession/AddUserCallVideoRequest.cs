namespace MessengerPrivate.Api.Models.CallSession
{
    public class AddUserCallVideoRequest
    {

        public string ConversationId { get; set; }

        public string CallSessionId { get; set; } 

        public bool? MuteCamera { get; set; } 

        public bool? MuteMicro { get; set; } 

        public string Status { get; set; }  

    }
}
