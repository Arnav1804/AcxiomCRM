using AxciomCRM.Models;

namespace AxciomCRM.ViewModels;

public class LeadListViewModel
{
    public List<Lead> Leads { get; set; } = new();
    public List<ApplicationUser> SalesUsers { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AssignedUser { get; set; } = string.Empty;
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
