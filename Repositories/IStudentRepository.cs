using CampusFlow.Mvc.Models;

namespace CampusFlow.Mvc.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();

    Task<List<Student>> SearchAsync(
        string? searchTerm,
        int? courseId);

    Task<Student?> GetByIdAsync(int id);

    Task AddAsync(Student student);

    Task UpdateAsync(Student student);

    Task DeleteAsync(int id);
}