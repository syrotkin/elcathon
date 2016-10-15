using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Services
{
    [Table("Sensor")]
    public class Sensor
    {
        [Key]
        [Column("SensorId")]
        public int SensorId { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("SensorType")]
        public string SensorType { get; set; }
    }
}