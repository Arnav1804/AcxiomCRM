using AxciomCRM.Models;

namespace AxciomCRM.ViewModels;

public class ActivityListViewModel
{
    public List<Activity> Activities { get; set; } = new();
    public List<ApplicationUser> Users { get; set; } = new();
    public string Type { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AssignedUser { get; set; } = string.Empty;
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
