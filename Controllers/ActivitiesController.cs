using AxciomCRM.Models;
using AxciomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AxciomCRM.Controllers;

[Authorize]
public class ActivitiesController : Controller
{
    private readonly ActivityService _activityService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuditService _auditService;

    public ActivitiesController(ActivityService activityService, UserManager<ApplicationUser> userManager, AuditService auditService)
    {
        _activityService = activityService;
        _userManager = userManager;
        _auditService = auditService;
    }

    public IActionResult Index(string? type, DateTime? date, string? status, string? assignedUser, int page = 1)
    {
        var userId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var model = _activityService.GetAll(type, date, status, assignedUser, page, userId);
        model.Users = _userManager.Users.Where(u => u.IsActive).ToList();
        return View(model);
    }

    public IActionResult Create()
    {
        ViewBag.Types = AppConstants.ActivityTypes;
        return View(new Activity { ActivityDate = DateTime.Now, AssignedTo = GetUserId() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Activity activity)
    {
        ModelState.Remove("AssignedTo");
        activity.AssignedTo = GetUserId();
        if (!AppConstants.ActivityTypes.Contains(activity.ActivityType))
        {
            ModelState.AddModelError("ActivityType", "Please select a valid activity type.");
        }

        if (!AppConstants.ActivityStatuses.Contains(activity.Status))
        {
            ModelState.AddModelError("Status", "Please select a valid activity status.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Types = AppConstants.ActivityTypes;
            return View(activity);
        }

        _activityService.Create(activity);
        _auditService.Log(GetUserId(), "Create", "Activity", activity.ActivityId.ToString(), null, activity.ActivityType + "; " + activity.Status);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var activity = _activityService.GetById(id);
        if (activity == null || (User.IsInRole(AppConstants.SalesExecutive) && activity.AssignedTo != GetUserId()))
        {
            return NotFound();
        }

        return View(activity);
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
