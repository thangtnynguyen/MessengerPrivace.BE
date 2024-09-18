namespace MessengerPrivate.Api.Models.Icon
{
    public class CreateIconRequest
    {
        public string? Type { get; set; }//image, svg,....

        public IFormFile? UrlFile { get; set; }

        public string? Data { get; set; }

        public string? Name { get; set; }
    }
}
