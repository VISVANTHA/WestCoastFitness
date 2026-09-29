using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Api.Data;
using WestCoastFitness.Api.Models;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/members")]
public class MembersController(FitnessClubDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll(CancellationToken cancellationToken)
    {
        var members = await dbContext.Members
            .OrderBy(m => m.FullName)
            .Select(m => new MemberDto(m.Id, m.FullName, m.Email, m.JoinedOnUtc, m.IsActive))
            .ToListAsync(cancellationToken);

        return Ok(members);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemberDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var member = await dbContext.Members.FindAsync([id], cancellationToken);
        if (member is null)
        {
            return NotFound();
        }

        return Ok(new MemberDto(member.Id, member.FullName, member.Email, member.JoinedOnUtc, member.IsActive));
    }

    [HttpPost]
    public async Task<ActionResult<MemberDto>> Create(CreateMemberRequest request, CancellationToken cancellationToken)
    {
        var emailInUse = await dbContext.Members.AnyAsync(m => m.Email == request.Email, cancellationToken);
        if (emailInUse)
        {
            return Conflict("A member with this email already exists.");
        }

        var member = new Member
        {
            FullName = request.FullName,
            Email = request.Email,
        };

        dbContext.Members.Add(member);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = new MemberDto(member.Id, member.FullName, member.Email, member.JoinedOnUtc, member.IsActive);
        return CreatedAtAction(nameof(GetById), new { id = member.Id }, dto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var member = await dbContext.Members.FindAsync([id], cancellationToken);
        if (member is null)
        {
            return NotFound();
        }

        member.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
