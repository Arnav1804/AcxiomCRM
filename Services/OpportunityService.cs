using AxciomCRM.Data;
using AxciomCRM.Models;
using AxciomCRM.ViewModels;

namespace AxciomCRM.Services;

public class OpportunityService
{
    private readonly ApplicationDbContext _context;

    public OpportunityService(ApplicationDbContext context)
    {
        _context = context;
    }

    public OpportunityListViewModel GetAll(string? name, int? customerId, string? stage, string? status, int page, string? assignedTo = null)
    {
        var opportunities = _context.Opportunities.ToList();

        if (!string.IsNullOrWhiteSpace(name))
        {
            opportunities = opportunities.Where(o => o.OpportunityName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (customerId.HasValue)
        {
            opportunities = opportunities.Where(o => o.CustomerId == customerId.Value).ToList();
        }

        if (!string.IsNullOrWhiteSpace(stage))
        {
            opportunities = opportunities.Where(o => o.Stage == stage).ToList();
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            opportunities = opportunities.Where(o => o.Status == status).ToList();
        }

        if (!string.IsNullOrEmpty(assignedTo))
        {
            opportunities = opportunities.Where(o => o.AssignedTo == assignedTo).ToList();
        }

        opportunities = opportunities.OrderByDescending(o => o.CreatedDate).ToList();
        var totalPages = (int)Math.Ceiling(opportunities.Count / 10.0);
        if (page < 1)
        {
            page = 1;
        }

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        return new OpportunityListViewModel
        {
            Opportunities = opportunities.Skip((page - 1) * 10).Take(10).ToList(),
            Name = name ?? string.Empty,
            CustomerId = customerId,
            Stage = stage ?? string.Empty,
            Status = status ?? string.Empty,
            Page = page,
            TotalPages = totalPages
        };
    }

    public List<Opportunity> GetAllList(string? assignedTo = null)
    {
        var opportunities = _context.Opportunities.ToList();
        if (!string.IsNullOrEmpty(assignedTo))
        {
            opportunities = opportunities.Where(o => o.AssignedTo == assignedTo).ToList();
        }

        return opportunities.OrderByDescending(o => o.CreatedDate).ToList();
    }

    public Opportunity? GetById(int id)
    {
        return _context.Opportunities.FirstOrDefault(o => o.OpportunityId == id);
    }

    public List<Customer> GetCustomers(string? createdBy = null)
    {
        var customers = _context.Customers.Where(c => c.Status == "Active").ToList();
        if (!string.IsNullOrEmpty(createdBy))
        {
            customers = customers.Where(c => c.CreatedBy == createdBy).ToList();
        }

        return customers.OrderBy(c => c.CustomerName).ToList();
    }

    public void Create(Opportunity opportunity)
    {
        _context.Opportunities.Add(opportunity);
        _context.SaveChanges();
    }

    public void Update(Opportunity opportunity)
    {
        _context.Opportunities.Update(opportunity);
        _context.SaveChanges();
    }

    public void Delete(Opportunity opportunity)
    {
        _context.Opportunities.Remove(opportunity);
        _context.SaveChanges();
    }

    public List<PipelineStageViewModel> GetPipeline(string? assignedTo = null)
    {
        var opportunities = _context.Opportunities.Where(o => o.Status == "Open").ToList();
        if (!string.IsNullOrEmpty(assignedTo))
        {
            opportunities = opportunities.Where(o => o.AssignedTo == assignedTo).ToList();
        }

        var pipeline = new List<PipelineStageViewModel>();
        foreach (var stage in AppConstants.OpportunityStages)
        {
            if (stage == "Won" || stage == "Lost")
            {
                continue;
            }

            var stageOpportunities = opportunities.Where(o => o.Stage == stage).ToList();
            pipeline.Add(new PipelineStageViewModel
            {
                Stage = stage,
                Count = stageOpportunities.Count,
                TotalAmount = stageOpportunities.Sum(o => o.Amount)
            });
        }

        return pipeline;
    }
}
