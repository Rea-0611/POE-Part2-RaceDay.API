using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models.DTOs.Results;
using RaceDay.API.Models.Entities;

namespace RaceDay.API.Services
{//start of namespace 

    public interface IResultService
    {//start of interface
        Task<List<ResultDto>> GetByEventAsync(int eventId);
    Task<List<ResultDto>> GetMyResultsAsync(int participantId);
    Task<ResultDto?> CreateAsync(int enrolmentId, CreateResultDto dto);
    Task<ResultDto?> UpdateAsync(int id, CreateResultDto dto);
    Task<bool> DeleteAsync(int id);
    }//end of interface

    public class ResultService : IResultService
    {//start of class
        private readonly RaceDayDbContext _context;

    public ResultService(RaceDayDbContext context)
    {
        _context = context;
    }

    public async Task<List<ResultDto>> GetByEventAsync(int eventId)
    {
        return await _context.Results
            .Where(r => r.Enrolment.Category.EventId == eventId)
            .Include(r => r.Enrolment).ThenInclude(en => en.Participant).ThenInclude(p => p.User)
            .Include(r => r.Enrolment).ThenInclude(en => en.Category)
            .Select(r => new ResultDto
            {
                ResultId = r.ResultId,
                ParticipantName = r.Enrolment.Participant.User.FullName,
                CategoryName = r.Enrolment.Category.CategoryName,
                FinishTime = r.FinishTime,
                OverallPosition = r.OverallPosition,
                CategoryPosition = r.CategoryPosition
            })
            .ToListAsync();
    }

    public async Task<List<ResultDto>> GetMyResultsAsync(int participantId)
    {
        return await _context.Results
            .Where(r => r.Enrolment.ParticipantId == participantId)
            .Include(r => r.Enrolment).ThenInclude(en => en.Participant).ThenInclude(p => p.User)
            .Include(r => r.Enrolment).ThenInclude(en => en.Category)
            .Select(r => new ResultDto
            {
                ResultId = r.ResultId,
                ParticipantName = r.Enrolment.Participant.User.FullName,
                CategoryName = r.Enrolment.Category.CategoryName,
                FinishTime = r.FinishTime,
                OverallPosition = r.OverallPosition,
                CategoryPosition = r.CategoryPosition
            })
            .ToListAsync();
    }

    public async Task<ResultDto?> CreateAsync(int enrolmentId, CreateResultDto dto)
    {
        var enrolment = await _context.Enrolments
            .Include(en => en.Participant).ThenInclude(p => p.User)
            .Include(en => en.Category)
            .FirstOrDefaultAsync(en => en.EnrolmentId == enrolmentId);
        if (enrolment == null) return null;

        var exists = await _context.Results.AnyAsync(r => r.EnrolmentId == enrolmentId);
        if (exists) return null;

        var result = new Result
        {
            EnrolmentId = enrolmentId,
            FinishTime = dto.FinishTime,
            OverallPosition = dto.OverallPosition,
            CategoryPosition = dto.CategoryPosition
        };

        _context.Results.Add(result);
        await _context.SaveChangesAsync();

        return new ResultDto
        {
            ResultId = result.ResultId,
            ParticipantName = enrolment.Participant.User.FullName,
            CategoryName = enrolment.Category.CategoryName,
            FinishTime = result.FinishTime,
            OverallPosition = result.OverallPosition,
            CategoryPosition = result.CategoryPosition
        };
    }

    public async Task<ResultDto?> UpdateAsync(int id, CreateResultDto dto)
    {
        var result = await _context.Results
            .Include(r => r.Enrolment).ThenInclude(en => en.Participant).ThenInclude(p => p.User)
            .Include(r => r.Enrolment).ThenInclude(en => en.Category)
            .FirstOrDefaultAsync(r => r.ResultId == id);
        if (result == null) return null;

        result.FinishTime = dto.FinishTime;
        result.OverallPosition = dto.OverallPosition;
        result.CategoryPosition = dto.CategoryPosition;

        await _context.SaveChangesAsync();

        return new ResultDto
        {
            ResultId = result.ResultId,
            ParticipantName = result.Enrolment.Participant.User.FullName,
            CategoryName = result.Enrolment.Category.CategoryName,
            FinishTime = result.FinishTime,
            OverallPosition = result.OverallPosition,
            CategoryPosition = result.CategoryPosition
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _context.Results.FindAsync(id);
        if (result == null) return false;

        _context.Results.Remove(result);
        await _context.SaveChangesAsync();
        return true;
    }

    }//end of class
}//end of namespace