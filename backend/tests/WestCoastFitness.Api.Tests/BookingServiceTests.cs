using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Api.Data;
using WestCoastFitness.Api.Models;
using WestCoastFitness.Api.Services;
using Xunit;

namespace WestCoastFitness.Api.Tests;

public class BookingServiceTests
{
    private static FitnessClubDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FitnessClubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FitnessClubDbContext(options);
    }

    [Fact]
    public async Task BookClassAsync_ReturnsBooked_WhenMemberAndClassAreValid()
    {
        using var db = CreateContext();
        var member = new Member { FullName = "Ada Lovelace", Email = "ada@example.com" };
        var classSession = new ClassSession { Title = "Spin", Instructor = "Jo", Capacity = 2, StartsAtUtc = DateTime.UtcNow };
        db.Members.Add(member);
        db.ClassSessions.Add(classSession);
        await db.SaveChangesAsync();

        var service = new BookingService(db);
        var result = await service.BookClassAsync(member.Id, classSession.Id);

        Assert.Equal(BookingOutcome.Booked, result.Outcome);
        Assert.NotNull(result.BookingId);
    }

    [Fact]
    public async Task BookClassAsync_ReturnsMemberNotFound_WhenMemberDoesNotExist()
    {
        using var db = CreateContext();
        var classSession = new ClassSession { Title = "Spin", Instructor = "Jo", Capacity = 2, StartsAtUtc = DateTime.UtcNow };
        db.ClassSessions.Add(classSession);
        await db.SaveChangesAsync();

        var service = new BookingService(db);
        var result = await service.BookClassAsync(memberId: 999, classSession.Id);

        Assert.Equal(BookingOutcome.MemberNotFound, result.Outcome);
    }

    [Fact]
    public async Task BookClassAsync_ReturnsMemberInactive_WhenMemberIsDeactivated()
    {
        using var db = CreateContext();
        var member = new Member { FullName = "Ada Lovelace", Email = "ada@example.com", IsActive = false };
        var classSession = new ClassSession { Title = "Spin", Instructor = "Jo", Capacity = 2, StartsAtUtc = DateTime.UtcNow };
        db.Members.Add(member);
        db.ClassSessions.Add(classSession);
        await db.SaveChangesAsync();

        var service = new BookingService(db);
        var result = await service.BookClassAsync(member.Id, classSession.Id);

        Assert.Equal(BookingOutcome.MemberInactive, result.Outcome);
    }

    [Fact]
    public async Task BookClassAsync_ReturnsClassNotFound_WhenClassDoesNotExist()
    {
        using var db = CreateContext();
        var member = new Member { FullName = "Ada Lovelace", Email = "ada@example.com" };
        db.Members.Add(member);
        await db.SaveChangesAsync();

        var service = new BookingService(db);
        var result = await service.BookClassAsync(member.Id, classSessionId: 999);

        Assert.Equal(BookingOutcome.ClassNotFound, result.Outcome);
    }

    [Fact]
    public async Task BookClassAsync_ReturnsAlreadyBooked_WhenMemberBooksSameClassTwice()
    {
        using var db = CreateContext();
        var member = new Member { FullName = "Ada Lovelace", Email = "ada@example.com" };
        var classSession = new ClassSession { Title = "Spin", Instructor = "Jo", Capacity = 2, StartsAtUtc = DateTime.UtcNow };
        db.Members.Add(member);
        db.ClassSessions.Add(classSession);
        await db.SaveChangesAsync();

        var service = new BookingService(db);
        await service.BookClassAsync(member.Id, classSession.Id);
        var result = await service.BookClassAsync(member.Id, classSession.Id);

        Assert.Equal(BookingOutcome.AlreadyBooked, result.Outcome);
    }

    [Fact]
    public async Task BookClassAsync_ReturnsClassFull_WhenCapacityReached()
    {
        using var db = CreateContext();
        var classSession = new ClassSession { Title = "Spin", Instructor = "Jo", Capacity = 1, StartsAtUtc = DateTime.UtcNow };
        var memberA = new Member { FullName = "Ada Lovelace", Email = "ada@example.com" };
        var memberB = new Member { FullName = "Grace Hopper", Email = "grace@example.com" };
        db.ClassSessions.Add(classSession);
        db.Members.AddRange(memberA, memberB);
        await db.SaveChangesAsync();

        var service = new BookingService(db);
        await service.BookClassAsync(memberA.Id, classSession.Id);
        var result = await service.BookClassAsync(memberB.Id, classSession.Id);

        Assert.Equal(BookingOutcome.ClassFull, result.Outcome);
    }
}
