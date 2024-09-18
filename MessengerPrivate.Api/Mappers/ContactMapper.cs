using AutoMapper;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.Contact;


namespace MessengerPrivate.Api.Mappers
{
    public class ContactMapper : Profile
    {
        public ContactMapper()
        {
            //CreateMap<CreateContactRequest, Contact>();

            CreateMap<Contact, ContactDto>();
            CreateMap<ContactDto, Contact>();
        }
    }
}
