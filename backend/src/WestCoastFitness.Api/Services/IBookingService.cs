namespace WestCoastFitness.Api.Services;

public interface IBookingService
{
    Task<BookingResult> BookClassAsync(int memberId, int classSessionId, CancellationToken cancellationToken = default);
}

public enum BookingOutcome
{
    Booked,
    MemberNotFound,
    MemberInactive,
    ClassNotFound,
    ClassFull,
    AlreadyBooked,
}

public record BookingResult(BookingOutcome Outcome, int? BookingId = null);
