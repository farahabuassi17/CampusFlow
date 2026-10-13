namespace CampusFlow.Mvc.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
public class Student
{
    public int StudentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? ImagePath { get; set; }
    public int CourseId { get; set; }

    [ValidateNever]
    public Course Course { get; set; } = null!;
    public List<Enrollment> Enrollments { get; set; } = new();
}