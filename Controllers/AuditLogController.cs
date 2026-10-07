using AxciomCRM.Data;
using AxciomCRM.Models;
using AxciomCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AxciomCRM.Controllers;

[Authorize(Roles = AppConstants.Admin)]
public class AuditLogController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuditLogController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index(string? userId, string? action, DateTime? fromDate, DateTime? toDate, int page = 1)
    {
        var logs = _context.AuditLogs.ToList();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            logs = logs.Where(a => a.UserId == userId).ToList();
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            logs = logs.Where(a => a.Action == action).ToList();
        }

        if (fromDate.HasValue)
        {
            logs = logs.Where(a => a.CreatedDate.Date >= fromDate.Value.Date).ToList();
        }

        if (toDate.HasValue)
        {
            logs = logs.Where(a => a.CreatedDate.Date <= toDate.Value.Date).ToList();
        }

        logs = logs.OrderByDescending(a => a.CreatedDate).ToList();
        var totalPages = (int)Math.Ceiling(logs.Count / 10.0);
        if (page < 1)
        {
            page = 1;
        }

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        return View(new AuditLogListViewModel
        {
            AuditLogs = logs.Skip((page - 1) * 10).Take(10).ToList(),
            Users = _userManager.Users.OrderBy(u => u.FullName).ToList(),
            Actions = _context.AuditLogs.Select(a => a.Action).Distinct().ToList(),
            UserId = userId ?? string.Empty,
            Action = action ?? string.Empty,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            TotalPages = totalPages
        });
    }
}
