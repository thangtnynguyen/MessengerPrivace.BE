
using MessengerPrivate.Api.Models.Common;

namespace MessengerPrivate.Api.Models.Auth
{
    public class GetUserRequest:PagingRequest
    {

        public bool? PhoneNumberConfirmed { get; set; }

        public DateTime? DeletedAt  { get; set; }

        public string? KeyWord { get; set; }

    }
}
