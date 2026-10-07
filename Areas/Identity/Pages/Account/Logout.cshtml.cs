using AxciomCRM.Models;
using AxciomCRM.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AxciomCRM.Areas.Identity.Pages.Account;

[ValidateAntiForgeryToken]
public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AuditService _auditService;

    public LogoutModel(SignInManager<ApplicationUser> signInManager, AuditService auditService)
    {
        _signInManager = signInManager;
        _auditService = auditService;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        _auditService.Log(userId, "Logout", "ApplicationUser", userId, null, "Successful logout");
        await _signInManager.SignOutAsync();
        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }
}
