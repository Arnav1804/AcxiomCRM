using AxciomCRM.Models;
using AxciomCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AxciomCRM.Controllers;

[Authorize(Roles = AppConstants.Admin)]
public class RolesController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RolesController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var roles = new[] { AppConstants.Admin, AppConstants.Manager, AppConstants.SalesExecutive };
        var model = new List<RoleCountViewModel>();
        foreach (var role in roles)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            model.Add(new RoleCountViewModel { Role = role, UserCount = users.Count });
        }

        return View(model);
    }
}
