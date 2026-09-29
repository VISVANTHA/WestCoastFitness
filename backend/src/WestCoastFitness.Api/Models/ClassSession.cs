using System.ComponentModel.DataAnnotations;

namespace WestCoastFitness.Api.Models;

public class ClassSession
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Instructor { get; set; } = string.Empty;

    public DateTime StartsAtUtc { get; set; }

    [Range(1, 100)]
    public int Capacity { get; set; } = 20;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
