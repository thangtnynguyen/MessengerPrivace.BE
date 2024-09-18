using AutoMapper;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.CallSession;
using MessengerPrivate.Api.Models.Contact;

namespace MessengerPrivate.Api.Mappers
{
    public class CallSessionMapper:Profile
    {

        public CallSessionMapper()
        {
            CreateMap<CreateCallSessionRequest, CallSession>();

            CreateMap<CallSession, CallSessionDto>();
            CreateMap<CallSessionDto, CallSession>();
        }
    }
}
