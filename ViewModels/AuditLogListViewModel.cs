using AxciomCRM.Models;

namespace AxciomCRM.ViewModels;

public class AuditLogListViewModel
{
    public List<AuditLog> AuditLogs { get; set; } = new();
    public List<ApplicationUser> Users { get; set; } = new();
    public List<string> Actions { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
