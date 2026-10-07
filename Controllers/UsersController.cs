using AxciomCRM.Models;
using AxciomCRM.Services;
using AxciomCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AxciomCRM.Controllers;

[Authorize(Roles = AppConstants.Admin)]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuditService _auditService;

    public UsersController(UserManager<ApplicationUser> userManager, AuditService auditService)
    {
        _userManager = userManager;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new List<UserRowViewModel>();
        foreach (var user in _userManager.Users.OrderBy(u => u.FullName).ToList())
        {
            var roles = await _userManager.GetRolesAsync(user);
            model.Add(new UserRowViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                IsActive = user.IsActive,
                IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now
            });
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        ViewBag.Roles = new[] { AppConstants.Admin, AppConstants.Manager, AppConstants.SalesExecutive };
        return View(new UserEditViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty,
            IsActive = user.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        if (!new[] { AppConstants.Admin, AppConstants.Manager, AppConstants.SalesExecutive }.Contains(model.Role))
        {
            ModelState.AddModelError("Role", "Please select a valid role.");
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.FullName = user.FullName;
            model.Email = user.Email ?? string.Empty;
            ViewBag.Roles = new[] { AppConstants.Admin, AppConstants.Manager, AppConstants.SalesExecutive };
            return View(model);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var oldRole = roles.FirstOrDefault() ?? string.Empty;
        if (oldRole != model.Role)
        {
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, model.Role);
            _auditService.Log(GetUserId(), "Role Change", "ApplicationUser", user.Id, oldRole, model.Role);
        }

        var oldStatus = user.IsActive ? "Active" : "Inactive";
        user.IsActive = model.IsActive;
        await _userManager.UpdateAsync(user);
        if (oldStatus != (user.IsActive ? "Active" : "Inactive"))
        {
            _auditService.Log(GetUserId(), "Status Change", "ApplicationUser", user.Id, oldStatus, user.IsActive ? "Active" : "Inactive");
        }
        return RedirectToAction(nameof(Index));
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
