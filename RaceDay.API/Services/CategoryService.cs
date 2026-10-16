using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models.DTOs.Categories;
using RaceDay.API.Models.Entities;

namespace RaceDay.API.Services
{//start of namespace 

    public interface ICategoryService
    {//start of interface
        Task<List<CategoryDto>> GetByEventAsync(int eventId);
    Task<CategoryDto?> CreateAsync(int eventId, CreateCategoryDto dto);
    Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(int id);
    }//end of interface

    public class CategoryService : ICategoryService
    {//start of class
        private readonly RaceDayDbContext _context;

    public CategoryService(RaceDayDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> GetByEventAsync(int eventId)
    {
        return await _context.Categories
            .Where(c => c.EventId == eventId)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryType = c.CategoryType,
                EntryFee = c.EntryFee
            })
            .ToListAsync();
    }

    public async Task<CategoryDto?> CreateAsync(int eventId, CreateCategoryDto dto)
    {
        var category = new Category
        {
            EventId = eventId,
            CategoryName = dto.CategoryName,
            CategoryType = dto.CategoryType,
            EntryFee = dto.EntryFee
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            CategoryType = category.CategoryType,
            EntryFee = category.EntryFee
        };
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return null;

        category.CategoryName = dto.CategoryName;
        category.CategoryType = dto.CategoryType;
        category.EntryFee = dto.EntryFee;

        await _context.SaveChangesAsync();

        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            CategoryType = category.CategoryType,
            EntryFee = category.EntryFee
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Enrolments)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return false;

        // Prevent delete if there are enrolments (NO ACTION FK)
        if (category.Enrolments.Any()) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}//end of class

}//end of namespace