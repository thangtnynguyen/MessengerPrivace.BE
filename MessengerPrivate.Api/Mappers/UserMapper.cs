using AutoMapper;
using MessengerPrivate.Api.Data.Entities;
using MessengerPrivate.Api.Models.User;


namespace MessengerPrivate.Api.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<CreateUserRequest, User>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
