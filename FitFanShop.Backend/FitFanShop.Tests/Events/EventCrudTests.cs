using System.Net;
using System.Net.Http.Json;
using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Events;
using FitFanShop.Application.Modules.Events.Commands.AddTicketType;
using FitFanShop.Application.Modules.Events.Commands.CreateEvent;
using FitFanShop.Application.Modules.Events.Commands.UpdateEvent;
using FitFanShop.Application.Modules.Events.Commands.UpdateTicketType;
using Xunit;

namespace FitFanShop.Tests.Events;

public class EventCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public EventCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Event_Create_CreatesNewEvent()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateEventCommand
        {
            Name = "Test Event",
            Description = "Test event description",
            EventDate = DateTime.UtcNow.AddDays(30),
            Location = "Test Stadium"
        };

        var response = await client.PostAsJsonAsync("/api/events", createCommand);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var eventDto = await response.Content.ReadFromJsonAsync<EventDetailsDto>();
        Assert.NotNull(eventDto);
        Assert.Equal("Test Event", eventDto.Name);
        Assert.Equal("Test Stadium", eventDto.Location);
    }

    [Fact]
    public async Task Event_GetAll_ReturnsEvents()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/events?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var events = await response.Content.ReadFromJsonAsync<PageResult<EventDto>>();
        Assert.NotNull(events);
        Assert.NotNull(events.Items);
    }

    [Fact]
    public async Task Event_GetUpcoming_ReturnsUpcomingEvents()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/events/upcoming");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var events = await response.Content.ReadFromJsonAsync<List<EventDto>>();
        Assert.NotNull(events);
    }

    [Fact]
    public async Task Event_GetById_ReturnsEvent()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateEventCommand
        {
            Name = "GetById Test Event",
            Description = "Test description",
            EventDate = DateTime.UtcNow.AddDays(20),
            Location = "Stadium"
        };

        var createResponse = await client.PostAsJsonAsync("/api/events", createCommand);
        var createdEvent = await createResponse.Content.ReadFromJsonAsync<EventDetailsDto>();

        var publicClient = _factory.CreateClient();
        var getResponse = await publicClient.GetAsync($"/api/events/{createdEvent!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var eventDto = await getResponse.Content.ReadFromJsonAsync<EventDetailsDto>();
        Assert.NotNull(eventDto);
        Assert.Equal("GetById Test Event", eventDto.Name);
    }

    [Fact]
    public async Task Event_Update_UpdatesEvent()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateEventCommand
        {
            Name = "Original Event",
            Description = "Original description",
            EventDate = DateTime.UtcNow.AddDays(15),
            Location = "Original Stadium"
        };

        var createResponse = await client.PostAsJsonAsync("/api/events", createCommand);
        var createdEvent = await createResponse.Content.ReadFromJsonAsync<EventDetailsDto>();

        var updateCommand = new UpdateEventCommand
        {
            Name = "Updated Event",
            Description = "Updated description",
            EventDate = DateTime.UtcNow.AddDays(25),
            Location = "Updated Stadium"
        };

        var updateResponse = await client.PutAsJsonAsync($"/api/events/{createdEvent!.Id}", updateCommand);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedEvent = await updateResponse.Content.ReadFromJsonAsync<EventDetailsDto>();
        Assert.NotNull(updatedEvent);
        Assert.Equal("Updated Event", updatedEvent.Name);
        Assert.Equal("Updated Stadium", updatedEvent.Location);
    }

    [Fact]
    public async Task Event_Delete_DeletesEvent()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateEventCommand
        {
            Name = "Event to Delete",
            Description = "Will be deleted",
            EventDate = DateTime.UtcNow.AddDays(10),
            Location = "Stadium"
        };

        var createResponse = await client.PostAsJsonAsync("/api/events", createCommand);
        var createdEvent = await createResponse.Content.ReadFromJsonAsync<EventDetailsDto>();

        var deleteResponse = await client.DeleteAsync($"/api/events/{createdEvent!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/events/{createdEvent.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Event_Create_ValidatesEventDate()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateEventCommand
        {
            Name = "Past Event",
            Description = "Event in the past",
            EventDate = DateTime.UtcNow.AddDays(-10),
            Location = "Stadium"
        };

        var response = await client.PostAsJsonAsync("/api/events", createCommand);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TicketType_Add_AddsTicketTypeToEvent()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createEventCommand = new CreateEventCommand
        {
            Name = "Event with Tickets",
            Description = "Test event",
            EventDate = DateTime.UtcNow.AddDays(30),
            Location = "Stadium"
        };

        var createEventResponse = await client.PostAsJsonAsync("/api/events", createEventCommand);
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<EventDetailsDto>();

        var addTicketTypeCommand = new AddTicketTypeCommand
        {
            Name = "VIP Ticket",
            Price = 150m,
            TotalAvailable = 50,
            Description = "VIP access"
        };

        var addTicketResponse = await client.PostAsJsonAsync(
            $"/api/events/{createdEvent!.Id}/ticket-types", 
            addTicketTypeCommand);
        Assert.Equal(HttpStatusCode.OK, addTicketResponse.StatusCode);

        var ticketType = await addTicketResponse.Content.ReadFromJsonAsync<TicketTypeDto>();
        Assert.NotNull(ticketType);
        Assert.Equal("VIP Ticket", ticketType.Name);
        Assert.Equal(150m, ticketType.Price);
    }

    [Fact]
    public async Task TicketType_Update_UpdatesTicketType()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createEventCommand = new CreateEventCommand
        {
            Name = "Event for Ticket Update",
            EventDate = DateTime.UtcNow.AddDays(30),
            Location = "Stadium"
        };

        var createEventResponse = await client.PostAsJsonAsync("/api/events", createEventCommand);
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<EventDetailsDto>();

        var addTicketTypeCommand = new AddTicketTypeCommand
        {
            Name = "Regular Ticket",
            Price = 40m,
            TotalAvailable = 100
        };

        var addTicketResponse = await client.PostAsJsonAsync(
            $"/api/events/{createdEvent!.Id}/ticket-types", 
            addTicketTypeCommand);
        var ticketType = await addTicketResponse.Content.ReadFromJsonAsync<TicketTypeDto>();

        var updateTicketTypeCommand = new UpdateTicketTypeCommand
        {
            Name = "Updated Regular Ticket",
            Price = 50m,
            TotalAvailable = 150
        };

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/events/{createdEvent.Id}/ticket-types/{ticketType!.Id}", 
            updateTicketTypeCommand);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedTicketType = await updateResponse.Content.ReadFromJsonAsync<TicketTypeDto>();
        Assert.NotNull(updatedTicketType);
        Assert.Equal("Updated Regular Ticket", updatedTicketType.Name);
        Assert.Equal(50m, updatedTicketType.Price);
    }

    [Fact]
    public async Task TicketType_Delete_DeletesTicketType()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createEventCommand = new CreateEventCommand
        {
            Name = "Event for Ticket Delete",
            EventDate = DateTime.UtcNow.AddDays(30),
            Location = "Stadium"
        };

        var createEventResponse = await client.PostAsJsonAsync("/api/events", createEventCommand);
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<EventDetailsDto>();

        var addTicketTypeCommand = new AddTicketTypeCommand
        {
            Name = "Ticket to Delete",
            Price = 30m,
            TotalAvailable = 50
        };

        var addTicketResponse = await client.PostAsJsonAsync(
            $"/api/events/{createdEvent!.Id}/ticket-types", 
            addTicketTypeCommand);
        var ticketType = await addTicketResponse.Content.ReadFromJsonAsync<TicketTypeDto>();

        var deleteResponse = await client.DeleteAsync(
            $"/api/events/{createdEvent.Id}/ticket-types/{ticketType!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getEventResponse = await client.GetAsync($"/api/events/{createdEvent.Id}");
        var eventDto = await getEventResponse.Content.ReadFromJsonAsync<EventDetailsDto>();
        Assert.NotNull(eventDto);
        Assert.DoesNotContain(eventDto.TicketTypes, tt => tt.Id == ticketType.Id);
    }

    [Fact]
    public async Task Event_Delete_CascadeDeletesTicketTypes()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createEventCommand = new CreateEventCommand
        {
            Name = "Event for Cascade Test",
            EventDate = DateTime.UtcNow.AddDays(30),
            Location = "Stadium"
        };

        var createEventResponse = await client.PostAsJsonAsync("/api/events", createEventCommand);
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<EventDetailsDto>();

        await client.PostAsJsonAsync(
            $"/api/events/{createdEvent!.Id}/ticket-types",
            new AddTicketTypeCommand { Name = "VIP", Price = 100m, TotalAvailable = 50 });

        await client.PostAsJsonAsync(
            $"/api/events/{createdEvent.Id}/ticket-types",
            new AddTicketTypeCommand { Name = "Regular", Price = 40m, TotalAvailable = 100 });

        var deleteEventResponse = await client.DeleteAsync($"/api/events/{createdEvent.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteEventResponse.StatusCode);

        var getEventResponse = await client.GetAsync($"/api/events/{createdEvent.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getEventResponse.StatusCode);
    }
}
