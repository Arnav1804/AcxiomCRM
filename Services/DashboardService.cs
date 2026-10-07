using AxciomCRM.Data;
using AxciomCRM.Models;
using AxciomCRM.ViewModels;

namespace AxciomCRM.Services;

public class DashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public DashboardViewModel GetDashboardData(string? userId = null)
    {
        var customers = _context.Customers.ToList();
        var leads = _context.Leads.ToList();
        var opportunities = _context.Opportunities.ToList();

        if (!string.IsNullOrEmpty(userId))
        {
            customers = customers.Where(c => c.CreatedBy == userId).ToList();
            leads = leads.Where(l => l.AssignedTo == userId).ToList();
            opportunities = opportunities.Where(o => o.AssignedTo == userId).ToList();
        }

        var model = new DashboardViewModel();

        model.TotalCustomers = customers.Count;
        model.TotalLeads = leads.Count;
        model.OpenLeads = leads.Where(l => l.Status != "Converted" && l.Status != "Lost" && l.Status != "Unqualified").Count();
        model.TotalOpportunities = opportunities.Count;
        model.OpenOpportunities = opportunities.Where(o => o.Status == "Open").Count();
        model.WonOpportunities = opportunities.Where(o => o.Status == "Won" || o.Stage == "Won").Count();
        model.LostOpportunities = opportunities.Where(o => o.Status == "Lost" || o.Stage == "Lost").Count();
        model.TotalPipelineValue = opportunities.Where(o => o.Status == "Open").Sum(o => o.Amount);

        // lead status chart
        foreach (var status in AppConstants.LeadStatuses)
        {
            model.LeadStatusLabels.Add(status);
            model.LeadStatusCounts.Add(leads.Count(l => l.Status == status));
        }

        // opportunity pipeline by stage chart
        foreach (var stage in AppConstants.OpportunityStages)
        {
            if (stage == "Won" || stage == "Lost")
            {
                continue;
            }

            model.PipelineStageLabels.Add(stage);
            var stageOpps = opportunities.Where(o => o.Stage == stage && o.Status == "Open").ToList();
            model.PipelineStageCounts.Add(stageOpps.Count);
            model.PipelineStageAmounts.Add(stageOpps.Sum(o => o.Amount));
        }

        // monthly sales chart for last 6 months
        var wonOpportunities = opportunities.Where(o => o.Status == "Won" || o.Stage == "Won").ToList();
        for (int i = 5; i >= 0; i--)
        {
            var targetMonth = DateTime.Today.AddMonths(-i);
            model.MonthlySalesLabels.Add(targetMonth.ToString("MMM yyyy"));

            var wonAmount = wonOpportunities
                .Where(o => o.ExpectedCloseDate.Year == targetMonth.Year && o.ExpectedCloseDate.Month == targetMonth.Month)
                .Sum(o => o.Amount);

            model.MonthlySalesAmounts.Add(wonAmount);
        }

        return model;
    }

    public int GetTotalCustomers(string? userId = null)
    {
        return GetDashboardData(userId).TotalCustomers;
    }

    public int GetTotalLeads(string? userId = null)
    {
        return GetDashboardData(userId).TotalLeads;
    }

    public int GetOpenLeads(string? userId = null)
    {
        return GetDashboardData(userId).OpenLeads;
    }

    public int GetTotalOpportunities(string? userId = null)
    {
        return GetDashboardData(userId).TotalOpportunities;
    }

    public int GetOpenOpportunities(string? userId = null)
    {
        return GetDashboardData(userId).OpenOpportunities;
    }

    public int GetWonOpportunities(string? userId = null)
    {
        return GetDashboardData(userId).WonOpportunities;
    }

    public int GetLostOpportunities(string? userId = null)
    {
        return GetDashboardData(userId).LostOpportunities;
    }

    public decimal GetTotalPipelineValue(string? userId = null)
    {
        return GetDashboardData(userId).TotalPipelineValue;
    }
}
