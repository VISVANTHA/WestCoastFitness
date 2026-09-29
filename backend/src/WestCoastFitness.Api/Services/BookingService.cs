using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Api.Data;
using WestCoastFitness.Api.Models;

namespace WestCoastFitness.Api.Services;

public class BookingService(FitnessClubDbContext dbContext) : IBookingService
{
    public async Task<BookingResult> BookClassAsync(int memberId, int classSessionId, CancellationToken cancellationToken = default)
    {
        var member = await dbContext.Members.FindAsync([memberId], cancellationToken);
        if (member is null)
        {
            return new BookingResult(BookingOutcome.MemberNotFound);
        }

        if (!member.IsActive)
        {
            return new BookingResult(BookingOutcome.MemberInactive);
        }

        var classSession = await dbContext.ClassSessions
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == classSessionId, cancellationToken);

        if (classSession is null)
        {
            return new BookingResult(BookingOutcome.ClassNotFound);
        }

        if (classSession.Bookings.Any(b => b.MemberId == memberId))
        {
            return new BookingResult(BookingOutcome.AlreadyBooked);
        }

        if (classSession.Bookings.Count >= classSession.Capacity)
        {
            return new BookingResult(BookingOutcome.ClassFull);
        }

        var booking = new Booking
        {
            MemberId = memberId,
            ClassSessionId = classSessionId,
        };

        dbContext.Bookings.Add(booking);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BookingResult(BookingOutcome.Booked, booking.Id);
    }
}
