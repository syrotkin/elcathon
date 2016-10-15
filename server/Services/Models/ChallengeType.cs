using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Services
{
    [Table("ChallengeType")]
    public class ChallengeType
    {
        [Key]
        [Column("ChallengeTypeId")]
        public int ChallengeTypeId { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public string SensorType { get; set; }

        public int DurationSeconds { get; set; }

        public int Bonus { get; set; }
    }

}