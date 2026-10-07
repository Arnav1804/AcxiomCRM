using AxciomCRM.Models;
using AxciomCRM.Services;
using AxciomCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AxciomCRM.Controllers;

[Authorize]
public class FollowUpsController : Controller
{
    private readonly FollowUpService _followUpService;
    private readonly CustomerService _customerService;
    private readonly LeadService _leadService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuditService _auditService;

    public FollowUpsController(FollowUpService followUpService, CustomerService customerService, LeadService leadService, UserManager<ApplicationUser> userManager, AuditService auditService)
    {
        _followUpService = followUpService;
        _customerService = customerService;
        _leadService = leadService;
        _userManager = userManager;
        _auditService = auditService;
    }

    public IActionResult Index(DateTime? date, string? status, string? assignedUser, int? customerId, int? leadId, int page = 1)
    {
        var userId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var model = _followUpService.GetAll(date, status, assignedUser, customerId, leadId, page, userId);
        LoadFilterLists(model, userId);
        return View(model);
    }

    public IActionResult Schedule()
    {
        LoadFormLists();
        return View(new FollowUp { FollowUpDate = DateTime.Today, AssignedTo = GetUserId() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Schedule(FollowUp followUp)
    {
        ModelState.Remove("AssignedTo");
        followUp.AssignedTo = GetUserId();
        CheckFollowUp(followUp);
        if (!ModelState.IsValid)
        {
            LoadFormLists();
            return View(followUp);
        }

        _followUpService.Create(followUp);
        _auditService.Log(GetUserId(), "Create", "FollowUp", followUp.FollowUpId.ToString(), null, Describe(followUp));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Complete(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var followUp = GetFollowUp(id);
        if (followUp == null)
        {
            return NotFound();
        }

        var oldValue = Describe(followUp);
        _followUpService.Complete(followUp);
        _auditService.Log(GetUserId(), "Update", "FollowUp", followUp.FollowUpId.ToString(), oldValue, Describe(followUp));
        return RedirectToAction(nameof(Index));
    }

    private FollowUp? GetFollowUp(int id)
    {
        var followUp = _followUpService.GetById(id);
        if (followUp != null && User.IsInRole(AppConstants.SalesExecutive) && followUp.AssignedTo != GetUserId())
        {
            return null;
        }

        return followUp;
    }

    private void CheckFollowUp(FollowUp followUp)
    {
        if (followUp.FollowUpDate.Date < DateTime.Today)
        {
            ModelState.AddModelError("FollowUpDate", "Follow-up date cannot be earlier than today.");
        }

        if (!AppConstants.FollowUpStatuses.Contains(followUp.Status))
        {
            ModelState.AddModelError("Status", "Please select a valid follow-up status.");
        }

        if (!followUp.CustomerId.HasValue && !followUp.LeadId.HasValue)
        {
            ModelState.AddModelError(string.Empty, "Please select a customer or lead.");
        }
    }

    private void LoadFormLists()
    {
        var userId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        ViewBag.Customers = _customerService.GetAll(null, 1, userId).Customers;
        ViewBag.Leads = _leadService.GetAll(null, null, null, null, 1, userId).Leads;
        ViewBag.Statuses = AppConstants.FollowUpStatuses;
        ViewBag.Types = AppConstants.ActivityTypes;
    }

    private void LoadFilterLists(FollowUpListViewModel model, string? userId)
    {
        model.Customers = _customerService.GetAll(null, 1, userId).Customers;
        model.Leads = _leadService.GetAll(null, null, null, null, 1, userId).Leads;
        model.Users = _userManager.Users.Where(u => u.IsActive).ToList();
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    private string Describe(FollowUp followUp)
    {
        return followUp.FollowUpDate.ToShortDateString() + "; " + followUp.FollowUpType + "; " + followUp.Status;
    }
}
