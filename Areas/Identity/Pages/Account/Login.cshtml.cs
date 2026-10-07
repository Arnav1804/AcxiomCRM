using AxciomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AxciomCRM.Services;
using System.ComponentModel.DataAnnotations;

namespace AxciomCRM.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuditService _auditService;

    public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuditService auditService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _auditService = auditService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect("~/Dashboard");
        }

        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null || !user.IsActive)
        {
            _auditService.Log(user?.Id ?? Input.Email, "Failed Login", "ApplicationUser", user?.Id ?? Input.Email, null, "Login not allowed");
            ModelState.AddModelError(string.Empty, "Login is not allowed for this account.");
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, true);
        if (result.Succeeded)
        {
            _auditService.Log(user.Id, "Login", "ApplicationUser", user.Id, null, "Successful login");
            return LocalRedirect(returnUrl ?? Url.Content("~/Dashboard"));
        }

        if (result.IsLockedOut)
        {
            _auditService.Log(user.Id, "Lockout", "ApplicationUser", user.Id, null, "Account locked after failed login");
            ModelState.AddModelError(string.Empty, "This account is locked. Please try again later.");
        }
        else
        {
            _auditService.Log(user.Id, "Failed Login", "ApplicationUser", user.Id, null, "Invalid password");
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return Page();
    }
}
