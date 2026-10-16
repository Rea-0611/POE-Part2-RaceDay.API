using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models.DTOs.Categories;
using RaceDay.API.Models.DTOs.Events;
using RaceDay.API.Models.Entities;

namespace RaceDay.API.Services
{//start of namespace

    public interface IEventService
    {//start of interface
        Task<List<EventDto>> GetAllAsync();
    Task<EventDto?> GetByIdAsync(int id);
    Task<EventDto> CreateAsync(CreateEventDto dto, int organiserId);
    Task<EventDto?> UpdateAsync(int id, UpdateEventDto dto, int organiserId);
    Task<bool> DeleteAsync(int id, int organiserId);
    Task<bool> OrganiserOwnsEvent(int eventId, int organiserId);
    }//end of interface

    public class EventService : IEventService
    {//start of class
        private readonly RaceDayDbContext _context;

    public EventService(RaceDayDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventDto>> GetAllAsync()
    {
        return await _context.Events
            .Include(e => e.Organiser).ThenInclude(o => o.User)
            .Include(e => e.Categories)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    public async Task<EventDto?> GetByIdAsync(int id)
    {
        var ev = await _context.Events
            .Include(e => e.Organiser).ThenInclude(o => o.User)
            .Include(e => e.Categories)
            .FirstOrDefaultAsync(e => e.EventId == id);

        return ev == null ? null : MapToDto(ev);
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto, int organiserId)
    {
        var ev = new Event
        {
            OrganiserId = organiserId,
            EventName = dto.EventName,
            EventDate = dto.EventDate,
            Location = dto.Location,
            Description = dto.Description,
            Distance = dto.Distance,
            EventType = dto.EventType,
            EventStatus = dto.EventStatus
        };

        _context.Events.Add(ev);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(ev.EventId) ?? throw new Exception("Event creation failed");
    }

    public async Task<EventDto?> UpdateAsync(int id, UpdateEventDto dto, int organiserId)
    {
        var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
        if (ev == null) return null;
        if (ev.OrganiserId != organiserId) return null;

        ev.EventName = dto.EventName;
        ev.EventDate = dto.EventDate;
        ev.Location = dto.Location;
        ev.Description = dto.Description;
        ev.Distance = dto.Distance;
        ev.EventType = dto.EventType;
        ev.EventStatus = dto.EventStatus;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id, int organiserId)
    {
        var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
        if (ev == null || ev.OrganiserId != organiserId) return false;

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> OrganiserOwnsEvent(int eventId, int organiserId)
    {
        return await _context.Events
            .AnyAsync(e => e.EventId == eventId && e.OrganiserId == organiserId);
    }

    private static EventDto MapToDto(Event e) => new()
    {
        EventId = e.EventId,
        EventName = e.EventName,
        EventDate = e.EventDate,
        Location = e.Location,
        Description = e.Description,
        Distance = e.Distance,
        EventType = e.EventType,
        EventStatus = e.EventStatus,
        OrganiserName = e.Organiser?.OrganisationName ?? string.Empty,
        Categories = e.Categories.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            CategoryType = c.CategoryType,
            EntryFee = c.EntryFee
        }).ToList()
    };
    }//end of class

}//end of namespace