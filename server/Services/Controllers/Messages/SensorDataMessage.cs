using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Controllers
{
    public class SensorDataMessage
    {
        public int SensorId { get; set; }

        public string SensorType { get; set; }

        public decimal Value { get; set; }
    }
}