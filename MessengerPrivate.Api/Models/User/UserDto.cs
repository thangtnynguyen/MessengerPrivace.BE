using Microsoft.AspNetCore.Identity;

namespace MessengerPrivate.Api.Models.User
{
    public class UserDto 
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public string ProfilePicture { get; set; }

        public string? Name { get; set; }

        public string? AvatarUrl { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public List<string> Permissions { get; set; }

        public List<int>? GroupModuleIds { get; set; }



    }
}
