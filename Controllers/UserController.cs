
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CampusFlow.Mvc.Models;
using CampusFlow.Mvc.Models.Enums;
using CampusFlow.Mvc.ViewModels;

namespace CampusFlow.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    // Display all active users
    public IActionResult Index()
    {
        var users = _userManager.Users
            .Where(u => !u.IsDelete)
            .ToList();

        var usersVm = _mapper.Map<List<UserViewModel>>(users);

        return View(usersVm);
    }

    // Display Create form
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Create a new user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _mapper.Map<User>(model);

        user.UserType = UserType.User;

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

    // Display Edit form

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var model = _mapper.Map<EditUserViewModel>(user);

        return View(model);
    }

    // Update user information
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

        _mapper.Map(model, user);

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

    // Soft delete a user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        // Prevent admin from deleting their own account
        if (user.Id == _userManager.GetUserId(User))
        {
            return BadRequest("You cannot delete your own account.");
        }

        // Prevent deletion of admin accounts
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
