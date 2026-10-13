using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusFlow.Mvc.Data;
using CampusFlow.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CampusFlow.Mvc.Controllers;

public class EnrollmentController : Controller
{
    private readonly ApplicationDbContext _context;

    public EnrollmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .ToListAsync();

        return View(enrollments);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var students = await _context.Students.ToListAsync();
        var courses = await _context.Courses.ToListAsync();

        ViewBag.Students = new SelectList(
            students, "StudentId", "Name");

        ViewBag.Courses = new SelectList(
            courses, "CourseId", "Name");

        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Enrollment enrollment)
    {
        ModelState.Remove("Student");
        ModelState.Remove("Course");

        if (ModelState.IsValid)
        {
            bool alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == enrollment.StudentId
                            && e.CourseId == enrollment.CourseId);

            if (alreadyEnrolled)
            {
                ModelState.AddModelError(
                    "",
                    "This student is already enrolled in this course.");
            }
            else
            {
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
        }

        ViewBag.Students = new SelectList(
            await _context.Students.ToListAsync(),
            "StudentId", "Name", enrollment.StudentId);

        ViewBag.Courses = new SelectList(
            await _context.Courses.ToListAsync(),
            "CourseId", "Name", enrollment.CourseId);

        return View(enrollment);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);

        if (enrollment == null)
        {
            return NotFound();
        }

        ViewBag.Students = new SelectList(
            await _context.Students.ToListAsync(),
            "StudentId",
            "Name",
            enrollment.StudentId);

        ViewBag.Courses = new SelectList(
            await _context.Courses.ToListAsync(),
            "CourseId",
            "Name",
            enrollment.CourseId);

        return View(enrollment);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Enrollment enrollment)
    {
        ModelState.Remove("Student");
        ModelState.Remove("Course");

        if (ModelState.IsValid)
        {
            var existingEnrollment = await _context.Enrollments
                .FindAsync(enrollment.EnrollmentId);

            if (existingEnrollment == null)
            {
                return NotFound();
            }

            bool alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == enrollment.StudentId
                            && e.CourseId == enrollment.CourseId
                            && e.EnrollmentId != enrollment.EnrollmentId);

            if (alreadyEnrolled)
            {
                ModelState.AddModelError(
                    "",
                    "This student is already enrolled in this course.");
            }
            else
            {
                existingEnrollment.StudentId = enrollment.StudentId;
                existingEnrollment.CourseId = enrollment.CourseId;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
        }

        ViewBag.Students = new SelectList(
            await _context.Students.ToListAsync(),
            "StudentId", "Name", enrollment.StudentId);

        ViewBag.Courses = new SelectList(
            await _context.Courses.ToListAsync(),
            "CourseId", "Name", enrollment.CourseId);

        return View(enrollment);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);

        if (enrollment == null)
        {
            return NotFound();
        }

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }



}