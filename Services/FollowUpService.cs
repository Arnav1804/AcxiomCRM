using AxciomCRM.Data;
using AxciomCRM.Models;
using AxciomCRM.ViewModels;

namespace AxciomCRM.Services;

public class FollowUpService
{
    private readonly ApplicationDbContext _context;

    public FollowUpService(ApplicationDbContext context)
    {
        _context = context;
    }

    public FollowUpListViewModel GetAll(DateTime? date, string? status, string? assignedUser, int? customerId, int? leadId, int page, string? salesUserId = null)
    {
        var followUps = _context.FollowUps.ToList();
        if (date.HasValue)
        {
            followUps = followUps.Where(f => f.FollowUpDate.Date == date.Value.Date).ToList();
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            followUps = followUps.Where(f => f.Status == status).ToList();
        }

        if (!string.IsNullOrWhiteSpace(assignedUser))
        {
            followUps = followUps.Where(f => f.AssignedTo == assignedUser).ToList();
        }

        if (customerId.HasValue)
        {
            followUps = followUps.Where(f => f.CustomerId == customerId).ToList();
        }

        if (leadId.HasValue)
        {
            followUps = followUps.Where(f => f.LeadId == leadId).ToList();
        }

        if (!string.IsNullOrEmpty(salesUserId))
        {
            followUps = followUps.Where(f => f.AssignedTo == salesUserId).ToList();
        }

        followUps = followUps.OrderBy(f => f.FollowUpDate).ToList();
        var totalPages = (int)Math.Ceiling(followUps.Count / 10.0);
        if (page < 1)
        {
            page = 1;
        }

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        return new FollowUpListViewModel
        {
            FollowUps = followUps.Skip((page - 1) * 10).Take(10).ToList(),
            Date = date,
            Status = status ?? string.Empty,
            AssignedUser = assignedUser ?? string.Empty,
            CustomerId = customerId,
            LeadId = leadId,
            Page = page,
            TotalPages = totalPages
        };
    }

    public FollowUp? GetById(int id)
    {
        return _context.FollowUps.FirstOrDefault(f => f.FollowUpId == id);
    }

    public void Create(FollowUp followUp)
    {
        _context.FollowUps.Add(followUp);
        _context.SaveChanges();
    }

    public void Complete(FollowUp followUp)
    {
        followUp.Status = "Completed";
        _context.SaveChanges();
    }
}
