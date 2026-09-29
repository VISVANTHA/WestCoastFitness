using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Api.Data;
using WestCoastFitness.Api.Models;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/classes")]
public class ClassSessionsController(FitnessClubDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassSessionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var classes = await dbContext.ClassSessions
            .Include(c => c.Bookings)
            .OrderBy(c => c.StartsAtUtc)
            .Select(c => new ClassSessionDto(c.Id, c.Title, c.Instructor, c.StartsAtUtc, c.Capacity, c.Bookings.Count))
            .ToListAsync(cancellationToken);

        return Ok(classes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClassSessionDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var classSession = await dbContext.ClassSessions
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (classSession is null)
        {
            return NotFound();
        }

        return Ok(new ClassSessionDto(classSession.Id, classSession.Title, classSession.Instructor,
            classSession.StartsAtUtc, classSession.Capacity, classSession.Bookings.Count));
    }

    [HttpPost]
    public async Task<ActionResult<ClassSessionDto>> Create(CreateClassSessionRequest request, CancellationToken cancellationToken)
    {
        var classSession = new ClassSession
        {
            Title = request.Title,
            Instructor = request.Instructor,
            StartsAtUtc = request.StartsAtUtc,
            Capacity = request.Capacity,
        };

        dbContext.ClassSessions.Add(classSession);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = new ClassSessionDto(classSession.Id, classSession.Title, classSession.Instructor,
            classSession.StartsAtUtc, classSession.Capacity, 0);
        return CreatedAtAction(nameof(GetById), new { id = classSession.Id }, dto);
    }
}
