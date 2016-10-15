using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Services
{
    [Table("SensorUser")]
    public class SensorUser
    {
        [Key]
        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        [Column("PasswordHash")]
        public string PasswordHash { get; set; }

        [Column("Score")]
        public int Score { get; set; }

    }
}