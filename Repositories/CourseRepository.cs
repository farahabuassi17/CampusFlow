using Microsoft.EntityFrameworkCore;
using CampusFlow.Mvc.Data;
using CampusFlow.Mvc.Models;

namespace CampusFlow.Mvc.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await _context.Courses
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Course>> SearchAsync(
        string? searchTerm,
        int? creditHours)
    {
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c =>
                c.Name.Contains(searchTerm));
        }

        if (creditHours.HasValue)
        {
            query = query.Where(c =>
                c.CreditHours == creditHours.Value);
        }

        return await query
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseId == id);
    }

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (course == null)
        {
            return;
        }

        bool hasStudents = await _context.Students
            .AnyAsync(s => s.CourseId == id);

        if (hasStudents)
        {
            throw new InvalidOperationException(
                "Cannot delete a course that has students assigned to it.");
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }
}