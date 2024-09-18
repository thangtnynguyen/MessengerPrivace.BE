using AutoMapper;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Contact;
using MessengerPrivate.Api.Models.Messenger;


namespace MessengerPrivate.Api.Mappers
{
    public class MessengerMapper : Profile
    {
        public MessengerMapper()
        {
            CreateMap<SendMessengerRequest, Messenger>();
            CreateMap<UpdateMessengerRequest, Messenger>();

            CreateMap<UpdateMessengerRequest, MessengerDto>();
            CreateMap<MessengerDto, UpdateMessengerRequest>();


            CreateMap<Messenger, MessengerDto>();
            CreateMap<MessengerDto, Messenger>();
        }
    }
}
