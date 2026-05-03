namespace TinyUrlApi.Models
{
    public class Url
    {
        public int Id { get; set; }

        public string? OriginalUrl { get; set; }

        public string? ShortCode { get; set; }

        public bool IsPrivate { get; set; }

        public int Clicks { get; set; }
    }
}
