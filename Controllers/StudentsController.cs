
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CampusFlow.Mvc.Repositories;
using CampusFlow.Mvc.Models;
using CampusFlow.Mvc.Services;

namespace CampusFlow.Mvc.Controllers;

public class StudentsController : Controller
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IWebHostEnvironment _environment;
    private readonly IEmailService _emailService;

    public StudentsController(
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        IWebHostEnvironment environment,
        IEmailService emailService
        )
    {
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _environment = environment;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchTerm,
        int? courseId)
    {
        // Get students using Student Repository
        var students = await _studentRepository.SearchAsync(
            searchTerm,
            courseId);

        // Get courses using Course Repository
        var courses = await _courseRepository.GetAllAsync();

        ViewBag.CourseList = new SelectList(
            courses,
            "CourseId",
            "Name",
            courseId);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.CourseId = courseId;

        return View(students);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCoursesAsync();

        return View(new Student());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Student student,
        IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            await LoadCoursesAsync();
            return View(student);
        }

        try
        {
            // Upload student image
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "students");

                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(imageFile.FileName);

                string filePath = Path.Combine(
                    uploadsFolder,
                    uniqueFileName);

                using (var fileStream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                student.ImagePath =
                    "/uploads/students/" + uniqueFileName;
            }

            // Save student to database
            await _studentRepository.AddAsync(student);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "Unable to create student.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            // Send automatic email
            await _emailService.SendEmailAsync(
                student.Email,
                "Welcome to CampusFlow",
                $"""
            <h2>Welcome to CampusFlow, {System.Net.WebUtility.HtmlEncode(student.Name)}!</h2>

            <p>Your student record has been created successfully.</p>

            <p>Thank you for using CampusFlow.</p>
            """
            );

            TempData["SuccessMessage"] =
                "Student created successfully and email sent!";
        }
        catch (Exception ex)
        {
            Console.WriteLine("EMAIL ERROR:");
            Console.WriteLine(ex.ToString());

            TempData["ErrorMessage"] =
                "Student was created, but there was a problem sending the email.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCoursesAsync()
    {
        var courses = await _courseRepository.GetAllAsync();

        ViewBag.CourseList = new SelectList(
            courses,
            "CourseId",
            "Name");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        var courses = await _courseRepository.GetAllAsync();

        ViewBag.CourseList = new SelectList(
            courses,
            "CourseId",
            "Name",
            student.CourseId);

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Student student,
        IFormFile? imageFile)
    {
        if (id != student.StudentId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            var courses = await _courseRepository.GetAllAsync();

            ViewBag.CourseList = new SelectList(
                courses,
                "CourseId",
                "Name",
                student.CourseId);

            return View(student);
        }

        try
        {
            var existingStudent =
                await _studentRepository.GetByIdAsync(student.StudentId);

            if (existingStudent == null)
            {
                return NotFound();
            }

            // Update student information
            existingStudent.Name = student.Name;
            existingStudent.Email = student.Email;
            existingStudent.CourseId = student.CourseId;

            // Upload new image only if selected
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "students");

                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(imageFile.FileName);

                string filePath = Path.Combine(
                    uploadsFolder,
                    uniqueFileName);

                using (var fileStream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                existingStudent.ImagePath =
                    "/uploads/students/" + uniqueFileName;
            }

            await _studentRepository.UpdateAsync(existingStudent);

            TempData["SuccessMessage"] =
                "Student updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "Unable to update the student. Please try again.";

            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        await _studentRepository.DeleteAsync(id);

        TempData["SuccessMessage"] =
            "Student deleted successfully!";

        return RedirectToAction(nameof(Index));
    }



}
