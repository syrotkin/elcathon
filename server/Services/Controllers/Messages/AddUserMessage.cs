

using Newtonsoft.Json;

namespace Services.Controllers
{
    public class AddUserMessage
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("passwordHash")]
        public string PasswordHash { get; set; }
    }
}