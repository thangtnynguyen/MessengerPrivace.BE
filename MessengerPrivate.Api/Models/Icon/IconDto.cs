using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MessengerPrivate.Api.Models.Icon
{
    public class IconDto
    {
        public string Id { get; set; }

        public string? Type { get; set; }//image, svg,....

        public string? Url { get; set; }

        public string? Data { get; set; }

        public string? Name { get; set; }
    }
}
