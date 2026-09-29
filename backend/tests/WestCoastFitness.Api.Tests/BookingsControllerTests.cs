using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WestCoastFitness.Api.Data;
using WestCoastFitness.Api.Models;
using Xunit;

namespace WestCoastFitness.Api.Tests;

public class BookingsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BookingsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbName = Guid.NewGuid().ToString();
                services.RemoveAll<DbContextOptions<FitnessClubDbContext>>();
                services.AddDbContext<FitnessClubDbContext>(options =>
                    options.UseInMemoryDatabase(dbName));
            });
        });
    }

    private async Task<(int MemberId, int ClassSessionId)> SeedMemberAndClassAsync(HttpClient client)
    {
        var memberResponse = await client.PostAsJsonAsync("/api/members",
            new CreateMemberRequest("Ada Lovelace", $"ada-{Guid.NewGuid()}@example.com"));
        var member = await memberResponse.Content.ReadFromJsonAsync<MemberDto>();

        var classResponse = await client.PostAsJsonAsync("/api/classes",
            new CreateClassSessionRequest("Spin", "Jo", DateTime.UtcNow.AddDays(1), 1));
        var classSession = await classResponse.Content.ReadFromJsonAsync<ClassSessionDto>();

        return (member!.Id, classSession!.Id);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenMemberAndClassAreValid()
    {
        var client = _factory.CreateClient();
        var (memberId, classSessionId) = await SeedMemberAndClassAsync(client);

        var response = await client.PostAsJsonAsync("/api/bookings", new CreateBookingRequest(memberId, classSessionId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenClassIsFull()
    {
        var client = _factory.CreateClient();
        var (memberId, classSessionId) = await SeedMemberAndClassAsync(client);
        await client.PostAsJsonAsync("/api/bookings", new CreateBookingRequest(memberId, classSessionId));

        var secondMemberResponse = await client.PostAsJsonAsync("/api/members",
            new CreateMemberRequest("Grace Hopper", $"grace-{Guid.NewGuid()}@example.com"));
        var secondMember = await secondMemberResponse.Content.ReadFromJsonAsync<MemberDto>();

        var response = await client.PostAsJsonAsync("/api/bookings", new CreateBookingRequest(secondMember!.Id, classSessionId));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        var client = _factory.CreateClient();
        var (_, classSessionId) = await SeedMemberAndClassAsync(client);

        var response = await client.PostAsJsonAsync("/api/bookings", new CreateBookingRequest(999999, classSessionId));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
