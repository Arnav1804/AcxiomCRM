using AxciomCRM.Models;
using AxciomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AxciomCRM.Controllers;

[Authorize]
public class LeadsController : Controller
{
    private readonly LeadService _leadService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuditService _auditService;

    public LeadsController(LeadService leadService, UserManager<ApplicationUser> userManager, AuditService auditService)
    {
        _leadService = leadService;
        _userManager = userManager;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index(string? name, string? company, string? status, string? assignedUser, int page = 1)
    {
        var salesUserId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var model = _leadService.GetAll(name, company, status, assignedUser, page, salesUserId);
        model.SalesUsers = await GetSalesUsers();
        return View(model);
    }

    public IActionResult Details(int id)
    {
        var lead = GetLead(id);
        if (lead == null)
        {
            return NotFound();
        }

        return View(lead);
    }

    public async Task<IActionResult> Create()
    {
        await LoadLists();
        return View(new Lead { AssignedTo = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lead lead)
    {
        ModelState.Remove("LeadCode");
        if (User.IsInRole(AppConstants.SalesExecutive))
        {
            ModelState.Remove("AssignedTo");
            lead.AssignedTo = GetUserId();
        }

        lead.CreatedDate = DateTime.Now;
        CheckStatus(lead.Status);
        await CheckAssignedUser(lead.AssignedTo);
        if (!ModelState.IsValid)
        {
            await LoadLists();
            return View(lead);
        }

        _leadService.Create(lead);
        _auditService.Log(GetUserId(), "Create", "Lead", lead.LeadId.ToString(), null, Describe(lead));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var lead = GetLead(id);
        if (lead == null)
        {
            return NotFound();
        }

        await LoadLists();
        return View(lead);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Lead lead)
    {
        ModelState.Remove("LeadCode");
        var existingLead = GetLead(id);
        if (existingLead == null)
        {
            return NotFound();
        }

        lead.LeadId = id;
        lead.LeadCode = existingLead.LeadCode;
        lead.CreatedDate = existingLead.CreatedDate;
        var oldValue = Describe(existingLead);
        if (User.IsInRole(AppConstants.SalesExecutive))
        {
            ModelState.Remove("AssignedTo");
            lead.AssignedTo = GetUserId();
        }

        CheckStatus(lead.Status);
        await CheckAssignedUser(lead.AssignedTo);
        if ((existingLead.Status == "Converted" || existingLead.Status == "Lost") && lead.Status != existingLead.Status)
        {
            ModelState.AddModelError("Status", "Converted or Lost leads cannot be changed to another status.");
        }

        if (!ModelState.IsValid)
        {
            await LoadLists();
            return View(lead);
        }

        _leadService.Update(lead);
        _auditService.Log(GetUserId(), "Update", "Lead", lead.LeadId.ToString(), oldValue, Describe(lead));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var lead = GetLead(id);
        if (lead == null)
        {
            return NotFound();
        }

        return View(lead);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var lead = GetLead(id);
        if (lead == null)
        {
            return NotFound();
        }

        var oldValue = Describe(lead);
        _leadService.Delete(lead);
        _auditService.Log(GetUserId(), "Delete", "Lead", id.ToString(), oldValue, null);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Convert(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var lead = GetLead(id);
        if (lead == null)
        {
            return NotFound();
        }

        if (lead.Status != "Qualified")
        {
            return BadRequest();
        }

        var oldValue = Describe(lead);
        _leadService.Convert(lead);
        _auditService.Log(GetUserId(), "Convert", "Lead", lead.LeadId.ToString(), oldValue, Describe(lead));
        return RedirectToAction(nameof(Details), new { id });
    }

    private Lead? GetLead(int id)
    {
        var lead = _leadService.GetById(id);
        if (lead != null && User.IsInRole(AppConstants.SalesExecutive) && lead.AssignedTo != GetUserId())
        {
            return null;
        }

        return lead;
    }

    private void CheckStatus(string status)
    {
        if (!AppConstants.LeadStatuses.Contains(status))
        {
            ModelState.AddModelError("Status", "Please select a valid lead status.");
        }
    }

    private async Task LoadLists()
    {
        ViewBag.Statuses = AppConstants.LeadStatuses;
        ViewBag.SalesUsers = await GetSalesUsers();
    }

    private async Task CheckAssignedUser(string assignedTo)
    {
        if (!User.IsInRole(AppConstants.SalesExecutive))
        {
            var users = await GetSalesUsers();
            if (!users.Any(u => u.Id == assignedTo))
            {
                ModelState.AddModelError("AssignedTo", "Please select a sales executive.");
            }
        }
    }

    private async Task<List<ApplicationUser>> GetSalesUsers()
    {
        var users = await _userManager.GetUsersInRoleAsync(AppConstants.SalesExecutive);
        return users.Where(u => u.IsActive).ToList();
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    private string Describe(Lead lead)
    {
        return lead.LeadName + "; " + lead.Status + "; " + lead.AssignedTo;
    }
}
