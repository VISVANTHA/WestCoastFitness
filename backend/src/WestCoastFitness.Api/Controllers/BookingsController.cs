using Microsoft.AspNetCore.Mvc;
using WestCoastFitness.Api.Models;
using WestCoastFitness.Api.Services;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var result = await bookingService.BookClassAsync(request.MemberId, request.ClassSessionId, cancellationToken);

        return result.Outcome switch
        {
            BookingOutcome.Booked => CreatedAtAction(nameof(Create), new { id = result.BookingId }, result),
            BookingOutcome.MemberNotFound => NotFound("Member not found."),
            BookingOutcome.ClassNotFound => NotFound("Class not found."),
            BookingOutcome.MemberInactive => Conflict("Member is not active."),
            BookingOutcome.AlreadyBooked => Conflict("Member already booked into this class."),
            BookingOutcome.ClassFull => Conflict("Class has reached capacity."),
            _ => BadRequest(),
        };
    }
}
