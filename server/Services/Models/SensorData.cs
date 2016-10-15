using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Models
{
    [Table("SensorData")]
    public class SensorData
    {
        [Key]
        [Column("SensorDataId")]
        public int SensorDataId { get; set; }

        [Column("SensorId")]
        public int SensorId { get; set; }

        [Column("Value")]
        public decimal Value { get; set; }

        [Column("Received")]
        public DateTime? Received { get; set; }

    }
}