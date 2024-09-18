using MessengerPrivate.Api.Models.Common;

namespace MessengerPrivate.Api.Models.Icon
{
    public class GetIconRequest: PagingRequest
    {
        public string? Name { get; set; }   

        public string? Type { get; set; }
    }
}
