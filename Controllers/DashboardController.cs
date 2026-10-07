using System.Security.Claims;
using System.Text.Json;
using AxciomCRM.Models;
using AxciomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxciomCRM.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public IActionResult Index()
    {
        var userId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var model = _dashboardService.GetDashboardData(userId);

        ViewBag.LeadStatusLabels = JsonSerializer.Serialize(model.LeadStatusLabels);
        ViewBag.LeadStatusCounts = JsonSerializer.Serialize(model.LeadStatusCounts);
        ViewBag.PipelineStageLabels = JsonSerializer.Serialize(model.PipelineStageLabels);
        ViewBag.PipelineStageCounts = JsonSerializer.Serialize(model.PipelineStageCounts);
        ViewBag.PipelineStageAmounts = JsonSerializer.Serialize(model.PipelineStageAmounts);
        ViewBag.MonthlySalesLabels = JsonSerializer.Serialize(model.MonthlySalesLabels);
        ViewBag.MonthlySalesAmounts = JsonSerializer.Serialize(model.MonthlySalesAmounts);

        return View(model);
    }

    [HttpGet]
    public IActionResult ChartData()
    {
        var userId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var model = _dashboardService.GetDashboardData(userId);
        return Json(new
        {
            leadStatus = new { labels = model.LeadStatusLabels, data = model.LeadStatusCounts },
            pipeline = new { labels = model.PipelineStageLabels, data = model.PipelineStageCounts, amounts = model.PipelineStageAmounts },
            monthlySales = new { labels = model.MonthlySalesLabels, data = model.MonthlySalesAmounts }
        });
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
