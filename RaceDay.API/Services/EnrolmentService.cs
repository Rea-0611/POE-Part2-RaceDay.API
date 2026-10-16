using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models.DTOs.Enrolments;
using RaceDay.API.Models.Entities;

namespace RaceDay.API.Services
{//start of namespace

    public interface IEnrolmentService
    {//start of interface
        Task<List<EnrolmentDto>> GetMyEnrolmentsAsync(int participantId);
    Task<List<EnrolmentDto>> GetByEventAsync(int eventId);
    Task<EnrolmentDto?> EnrolAsync(int participantId, int categoryId, EnrolDto dto);
    Task<bool> UpdatePaymentAsync(int enrolmentId, string newStatus, int userId, string role);
    }//end of interface

    public class EnrolmentService : IEnrolmentService
    { //start of class
        private readonly RaceDayDbContext _context;

    public EnrolmentService(RaceDayDbContext context)
    {
        _context = context;
    }

    public async Task<List<EnrolmentDto>> GetMyEnrolmentsAsync(int participantId)
    {
        return await _context.Enrolments
            .Where(en => en.ParticipantId == participantId)
            .Include(en => en.Participant).ThenInclude(p => p.User)
            .Include(en => en.Category).ThenInclude(c => c.Event)
            .Select(en => new EnrolmentDto
            {
                EnrolmentId = en.EnrolmentId,
                ParticipantName = en.Participant.User.FullName,
                EventName = en.Category.Event.EventName,
                CategoryName = en.Category.CategoryName,
                EnrolmentDate = en.EnrolmentDate,
                PaymentStatus = en.PaymentStatus
            })
            .ToListAsync();
    }

    public async Task<List<EnrolmentDto>> GetByEventAsync(int eventId)
    {
        return await _context.Enrolments
            .Where(en => en.Category.EventId == eventId)
            .Include(en => en.Participant).ThenInclude(p => p.User)
            .Include(en => en.Category).ThenInclude(c => c.Event)
            .Select(en => new EnrolmentDto
            {
                EnrolmentId = en.EnrolmentId,
                ParticipantName = en.Participant.User.FullName,
                EventName = en.Category.Event.EventName,
                CategoryName = en.Category.CategoryName,
                EnrolmentDate = en.EnrolmentDate,
                PaymentStatus = en.PaymentStatus
            })
            .ToListAsync();
    }

    public async Task<EnrolmentDto?> EnrolAsync(int participantId, int categoryId, EnrolDto dto)
    {
        // Check for duplicate
        var exists = await _context.Enrolments
            .AnyAsync(en => en.ParticipantId == participantId && en.CategoryId == categoryId);
        if (exists) return null;

        var category = await _context.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        if (category == null) return null;

        var enrolment = new Enrolment
        {
            ParticipantId = participantId,
            CategoryId = categoryId,
            PaymentStatus = dto.PaymentStatus
        };

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        var participant = await _context.Participants
            .Include(p => p.User)
            .FirstAsync(p => p.ParticipantId == participantId);

        return new EnrolmentDto
        {
            EnrolmentId = enrolment.EnrolmentId,
            ParticipantName = participant.User.FullName,
            EventName = category.Event.EventName,
            CategoryName = category.CategoryName,
            EnrolmentDate = enrolment.EnrolmentDate,
            PaymentStatus = enrolment.PaymentStatus
        };
    }

    public async Task<bool> UpdatePaymentAsync(int enrolmentId, string newStatus, int userId, string role)
    {
        var enrolment = await _context.Enrolments
            .Include(en => en.Participant)
            .Include(en => en.Category).ThenInclude(c => c.Event)
            .FirstOrDefaultAsync(en => en.EnrolmentId == enrolmentId);

        if (enrolment == null) return false;

        // Participant can only update their own; Organiser must own the event
        if (role == "Participant" && enrolment.ParticipantId != userId) return false;
        if (role == "Organiser" && enrolment.Category.Event.OrganiserId != userId) return false;

        if (newStatus != "Pending" && newStatus != "Paid") return false;

        enrolment.PaymentStatus = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }
    }//end of class

}//end of namespace
