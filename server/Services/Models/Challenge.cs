using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Models
{
    [Table("Challenge")]
    public class Challenge
    {
        [Key]
        [Column("ChallengeId")]
        public int ChallengeId { get; set; }

        public int ChallengeTypeId { get; set; }

        public int OwnerId { get; set; }

        public int VictimId { get; set; }

        public int SensorId { get; set; }

        public DateTime StartDate { get; set; }
        public string Status { get; set; }
    }

}