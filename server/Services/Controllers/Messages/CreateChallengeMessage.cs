namespace Services.Controllers
{
    public class CreateChallengeMessage
    {
        public int OwnerId { get; set; }

        public int VictimId { get; set; }

        public string ChallengeType { get; set; }
    }
}