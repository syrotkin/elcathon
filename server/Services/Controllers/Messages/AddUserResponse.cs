using Newtonsoft.Json;

namespace Services.Controllers
{
    public class AddUserResponse
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }
    }
}