using AxciomCRM.Models;
using AxciomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AxciomCRM.Controllers;

[Authorize]
public class OpportunitiesController : Controller
{
    private readonly OpportunityService _opportunityService;
    private readonly AuditService _auditService;

    public OpportunitiesController(OpportunityService opportunityService, AuditService auditService)
    {
        _opportunityService = opportunityService;
        _auditService = auditService;
    }

    public IActionResult Index(string? name, int? customerId, string? stage, string? status, int page = 1)
    {
        var userId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var model = _opportunityService.GetAll(name, customerId, stage, status, page, userId);
        model.Customers = _opportunityService.GetCustomers(userId);
        return View(model);
    }

    public IActionResult Details(int id)
    {
        var opportunity = GetOpportunity(id);
        if (opportunity == null)
        {
            return NotFound();
        }

        ViewBag.Customer = _opportunityService.GetCustomers().FirstOrDefault(c => c.CustomerId == opportunity.CustomerId);
        return View(opportunity);
    }

    public IActionResult Create()
    {
        LoadCustomers();
        return View(new Opportunity { ExpectedCloseDate = DateTime.Today.AddDays(30) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Opportunity opportunity)
    {
        ModelState.Remove("AssignedTo");
        opportunity.AssignedTo = GetUserId();
        opportunity.CreatedDate = DateTime.Now;
        SetStatus(opportunity);
        CheckRules(opportunity);
        CheckCustomer(opportunity.CustomerId);
        if (!ModelState.IsValid)
        {
            LoadCustomers();
            return View(opportunity);
        }

        _opportunityService.Create(opportunity);
        _auditService.Log(GetUserId(), "Create", "Opportunity", opportunity.OpportunityId.ToString(), null, Describe(opportunity));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var opportunity = GetOpportunity(id);
        if (opportunity == null)
        {
            return NotFound();
        }

        LoadCustomers();
        return View(opportunity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Opportunity opportunity)
    {
        ModelState.Remove("AssignedTo");
        var existingOpportunity = GetOpportunity(id);
        if (existingOpportunity == null)
        {
            return NotFound();
        }

        opportunity.OpportunityId = id;
        opportunity.LeadId = existingOpportunity.LeadId;
        opportunity.AssignedTo = existingOpportunity.AssignedTo;
        opportunity.CreatedDate = existingOpportunity.CreatedDate;
        var oldValue = Describe(existingOpportunity);
        SetStatus(opportunity);
        CheckRules(opportunity);
        CheckCustomer(opportunity.CustomerId);
        if (!ModelState.IsValid)
        {
            LoadCustomers();
            return View(opportunity);
        }

        _opportunityService.Update(opportunity);
        _auditService.Log(GetUserId(), "Update", "Opportunity", opportunity.OpportunityId.ToString(), oldValue, Describe(opportunity));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var opportunity = GetOpportunity(id);
        if (opportunity == null)
        {
            return NotFound();
        }

        return View(opportunity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var opportunity = GetOpportunity(id);
        if (opportunity == null)
        {
            return NotFound();
        }

        var oldValue = Describe(opportunity);
        _opportunityService.Delete(opportunity);
        _auditService.Log(GetUserId(), "Delete", "Opportunity", id.ToString(), oldValue, null);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Pipeline()
    {
        var userId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        return View(_opportunityService.GetPipeline(userId));
    }

    private Opportunity? GetOpportunity(int id)
    {
        var opportunity = _opportunityService.GetById(id);
        if (opportunity != null && User.IsInRole(AppConstants.SalesExecutive) && opportunity.AssignedTo != GetUserId())
        {
            return null;
        }

        return opportunity;
    }

    private void LoadCustomers()
    {
        var userId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        ViewBag.Customers = _opportunityService.GetCustomers(userId);
        ViewBag.Stages = AppConstants.OpportunityStages;
    }

    private void SetStatus(Opportunity opportunity)
    {
        if (opportunity.Stage == "Won")
        {
            opportunity.Status = "Won";
        }
        else if (opportunity.Stage == "Lost")
        {
            opportunity.Status = "Lost";
        }
        else
        {
            opportunity.Status = "Open";
        }
    }

    private void CheckRules(Opportunity opportunity)
    {
        if (!AppConstants.OpportunityStages.Contains(opportunity.Stage))
        {
            ModelState.AddModelError("Stage", "Please select a valid stage.");
        }

        if (opportunity.Amount <= 0)
        {
            ModelState.AddModelError("Amount", "Opportunity Amount must be greater than 0.");
        }

        if (opportunity.Probability < 0 || opportunity.Probability > 100)
        {
            ModelState.AddModelError("Probability", "Probability must be between 0 and 100.");
        }

        if (opportunity.Status == "Open" && opportunity.ExpectedCloseDate.Date < DateTime.Today)
        {
            ModelState.AddModelError("ExpectedCloseDate", "Expected Close Date cannot be in the past.");
        }
    }

    private void CheckCustomer(int? customerId)
    {
        var userId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var customers = _opportunityService.GetCustomers(userId);
        if (!customerId.HasValue || !customers.Any(c => c.CustomerId == customerId.Value))
        {
            ModelState.AddModelError("CustomerId", "Please select a valid customer.");
        }
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    private string Describe(Opportunity opportunity)
    {
        return opportunity.OpportunityName + "; " + opportunity.Stage + "; " + opportunity.Amount;
    }
}
