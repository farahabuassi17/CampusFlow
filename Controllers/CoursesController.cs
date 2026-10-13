using Microsoft.AspNetCore.Mvc;
using CampusFlow.Mvc.Models;
using CampusFlow.Mvc.Repositories;

namespace CampusFlow.Mvc.Controllers;

public class CoursesController : Controller
{
    private readonly ICourseRepository _courseRepository;

    public CoursesController(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IActionResult> Index(
        string? searchTerm,
        int? creditHours)
    {
        var courses = await _courseRepository.SearchAsync(
            searchTerm,
            creditHours);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.CreditHours = creditHours;

        return View(courses);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Course());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid)
        {
            return View(course);
        }

        await _courseRepository.AddAsync(course);

        TempData["SuccessMessage"] =
            "Course created successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.CourseId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(course);
        }

        await _courseRepository.UpdateAsync(course);

        TempData["SuccessMessage"] =
            "Course updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }


    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(id);

            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction(nameof(Index));
            }

            await _courseRepository.DeleteAsync(id);

            TempData["SuccessMessage"] =
                "Course deleted successfully!";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "Unable to delete this course. It may have students assigned to it.";
        }

        return RedirectToAction(nameof(Index));
    }




}