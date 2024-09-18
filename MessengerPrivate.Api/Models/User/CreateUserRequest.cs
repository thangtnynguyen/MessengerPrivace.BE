using Newtonsoft.Json;

namespace MessengerPrivate.Api.Models.User
{
    public class CreateUserRequest
    {
        public string? Name { get; set; }

        public IFormFile? AvatarFile { get; set; }

        [JsonIgnore]
        public string? AvatarUrl { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Password { get; set; }

        public bool? Status { get; set; }

        public string? Role { get; set; }
    }
}
