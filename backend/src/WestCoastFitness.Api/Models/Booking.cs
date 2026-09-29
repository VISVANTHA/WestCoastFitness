namespace WestCoastFitness.Api.Models;

public class Booking
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public Member? Member { get; set; }

    public int ClassSessionId { get; set; }
    public ClassSession? ClassSession { get; set; }

    public DateTime BookedAtUtc { get; set; } = DateTime.UtcNow;
}
