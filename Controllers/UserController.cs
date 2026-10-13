using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CampusFlow.Mvc.Models;
using CampusFlow.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CampusFlow.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        var users = _userManager.Users
            .Where(u => !u.IsDelete)
            .Select(u => new UserViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                IsDelete = u.IsDelete,
                UserType = u.UserType
            })
            .ToList();


        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            UserType = CampusFlow.Mvc.Models.Enums.UserType.User
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (roleResult.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var model = new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.Id);

        if (user == null)
        {
            return NotFound();
        }

        user.UserName = model.UserName;
        user.Email = model.Email;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        if (user.Id == _userManager.GetUserId(User))
        {
            return BadRequest("You cannot delete your own account.");
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return BadRequest("Admin accounts cannot be deleted.");
        }

        user.IsDelete = true;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }

        return BadRequest("Failed to delete user.");
    }

}
