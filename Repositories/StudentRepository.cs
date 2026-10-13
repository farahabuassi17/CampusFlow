using Microsoft.EntityFrameworkCore;
using CampusFlow.Mvc.Data;
using CampusFlow.Mvc.Models;

namespace CampusFlow.Mvc.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students
            .Include(s => s.Course)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<Student>> SearchAsync(
        string? searchTerm,
        int? courseId)
    {
        var query = _context.Students
            .Include(s => s.Course)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s =>
                s.Name.Contains(searchTerm) ||
                s.Email.Contains(searchTerm));
        }

        if (courseId.HasValue)
        {
            query = query.Where(s =>
                s.CourseId == courseId.Value);
        }

        return await query
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.StudentId == id);
    }

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}