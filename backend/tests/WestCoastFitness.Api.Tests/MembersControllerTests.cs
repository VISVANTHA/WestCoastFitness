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

public class MembersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MembersControllerTests(WebApplicationFactory<Program> factory)
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

    [Fact]
    public async Task Create_ThenGetAll_ReturnsCreatedMember()
    {
        var client = _factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/members",
            new CreateMemberRequest("Ada Lovelace", "ada@example.com"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var getAllResponse = await client.GetAsync("/api/members");
        getAllResponse.EnsureSuccessStatusCode();
        var members = await getAllResponse.Content.ReadFromJsonAsync<List<MemberDto>>();

        Assert.Contains(members!, m => m.Email == "ada@example.com");
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenEmailAlreadyExists()
    {
        var client = _factory.CreateClient();
        var request = new CreateMemberRequest("Ada Lovelace", "duplicate@example.com");

        await client.PostAsJsonAsync("/api/members", request);
        var secondResponse = await client.PostAsJsonAsync("/api/members", request);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForUnknownMember()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/members/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Deactivate_SetsMemberInactive()
    {
        var client = _factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/members",
            new CreateMemberRequest("Grace Hopper", "grace@example.com"));
        var created = await createResponse.Content.ReadFromJsonAsync<MemberDto>();

        var deleteResponse = await client.DeleteAsync($"/api/members/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/members/{created.Id}");
        var member = await getResponse.Content.ReadFromJsonAsync<MemberDto>();
        Assert.False(member!.IsActive);
    }

    [Fact]
    public async Task Deactivate_ReturnsNotFound_ForUnknownMember()
    {
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/api/members/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
