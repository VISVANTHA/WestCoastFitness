using System.ComponentModel.DataAnnotations;

namespace WestCoastFitness.Api.Models;

public record MemberDto(int Id, string FullName, string Email, DateTime JoinedOnUtc, bool IsActive);

public record CreateMemberRequest(
    [Required, MaxLength(100)] string FullName,
    [Required, EmailAddress, MaxLength(200)] string Email);

public record ClassSessionDto(int Id, string Title, string Instructor, DateTime StartsAtUtc, int Capacity, int BookedCount);

public record CreateClassSessionRequest(
    [Required, MaxLength(100)] string Title,
    [Required, MaxLength(100)] string Instructor,
    DateTime StartsAtUtc,
    [Range(1, 100)] int Capacity);

public record CreateBookingRequest(int MemberId, int ClassSessionId);
