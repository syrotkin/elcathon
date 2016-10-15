using Newtonsoft.Json;

namespace Services.Controllers
{
    public class AddSensorMessage
    {
        [JsonProperty("sensorType")]
        public string SensorType { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }
    }
}