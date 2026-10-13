namespace CampusFlow.Mvc.Models;

public class Enrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public Student Student { get; set; } = null!;

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;
}