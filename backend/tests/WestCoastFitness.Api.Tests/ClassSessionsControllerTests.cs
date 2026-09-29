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

public class ClassSessionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ClassSessionsControllerTests(WebApplicationFactory<Program> factory)
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
    public async Task Create_ThenGetById_ReturnsCreatedClass()
    {
        var client = _factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/classes",
            new CreateClassSessionRequest("Spin", "Jo", DateTime.UtcNow.AddDays(1), 15));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<ClassSessionDto>();

        var getResponse = await client.GetAsync($"/api/classes/{created!.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<ClassSessionDto>();

        Assert.Equal("Spin", fetched!.Title);
        Assert.Equal(0, fetched.BookedCount);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForUnknownClass()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/classes/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsClassesOrderedByStartTime()
    {
        var client = _factory.CreateClient();
        var later = DateTime.UtcNow.AddDays(2);
        var sooner = DateTime.UtcNow.AddDays(1);

        await client.PostAsJsonAsync("/api/classes", new CreateClassSessionRequest("Yoga", "Sam", later, 10));
        await client.PostAsJsonAsync("/api/classes", new CreateClassSessionRequest("Spin", "Jo", sooner, 10));

        var response = await client.GetAsync("/api/classes");
        var classes = await response.Content.ReadFromJsonAsync<List<ClassSessionDto>>();

        Assert.True(classes!.Count >= 2);
        Assert.True(classes[0].StartsAtUtc <= classes[1].StartsAtUtc);
    }
}
