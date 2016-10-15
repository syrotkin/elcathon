using Newtonsoft.Json;

namespace Services.Controllers
{
    public class AddSensorResponse
    {
        [JsonProperty("sensorId")]
        public int SensorId { get; set; }
    }
}