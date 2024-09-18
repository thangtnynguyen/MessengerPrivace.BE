using AutoMapper;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Conversation;
using MessengerPrivate.Api.Models.Messenger;

namespace MessengerPrivate.Api.Mappers
{
    public class ConversationMapper:Profile
    {

        public ConversationMapper()
        {

            CreateMap<CreateConversationRequest, Conversation>();
            CreateMap<UpdateInfoConversationRequest, Conversation>();

            //CreateMap<UpdateMessengerRequest, MessengerDto>();
            //CreateMap<MessengerDto, UpdateMessengerRequest>();


            CreateMap<Conversation, ConversationDto>();
            CreateMap<ConversationDto, Conversation>();

        }

       
    }
}
