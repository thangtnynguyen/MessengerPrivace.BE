using AutoMapper;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Contact;
using MessengerPrivate.Api.Models.Icon;
using MessengerPrivate.Api.Models.Messenger;

namespace MessengerPrivate.Api.Mappers
{
    public class IconMapper:Profile
    {
        public IconMapper()
        {
            CreateMap<CreateIconRequest, Icon>();
            CreateMap<Icon, IconDto>();
            CreateMap<IconDto, Icon>();

            CreateMap<IconRequest, IconDto>();
        }

    }
}
