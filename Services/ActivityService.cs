using AxciomCRM.Data;
using AxciomCRM.Models;
using AxciomCRM.ViewModels;

namespace AxciomCRM.Services;

public class ActivityService
{
    private readonly ApplicationDbContext _context;

    public ActivityService(ApplicationDbContext context)
    {
        _context = context;
    }

    public ActivityListViewModel GetAll(string? type, DateTime? date, string? status, string? assignedUser, int page, string? salesUserId = null)
    {
        var activities = _context.Activities.ToList();
        if (!string.IsNullOrWhiteSpace(type))
        {
            activities = activities.Where(a => a.ActivityType == type).ToList();
        }

        if (date.HasValue)
        {
            activities = activities.Where(a => a.ActivityDate.Date == date.Value.Date).ToList();
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            activities = activities.Where(a => a.Status == status).ToList();
        }

        if (!string.IsNullOrWhiteSpace(assignedUser))
        {
            activities = activities.Where(a => a.AssignedTo == assignedUser).ToList();
        }

        if (!string.IsNullOrEmpty(salesUserId))
        {
            activities = activities.Where(a => a.AssignedTo == salesUserId).ToList();
        }

        activities = activities.OrderByDescending(a => a.ActivityDate).ToList();
        var totalPages = (int)Math.Ceiling(activities.Count / 10.0);
        if (page < 1)
        {
            page = 1;
        }

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        return new ActivityListViewModel
        {
            Activities = activities.Skip((page - 1) * 10).Take(10).ToList(),
            Type = type ?? string.Empty,
            Date = date,
            Status = status ?? string.Empty,
            AssignedUser = assignedUser ?? string.Empty,
            Page = page,
            TotalPages = totalPages
        };
    }

    public Activity? GetById(int id)
    {
        return _context.Activities.FirstOrDefault(a => a.ActivityId == id);
    }

    public void Create(Activity activity)
    {
        _context.Activities.Add(activity);
        _context.SaveChanges();
    }
}
