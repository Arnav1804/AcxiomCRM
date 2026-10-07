using AxciomCRM.Models;

namespace AxciomCRM.ViewModels;

public class FollowUpListViewModel
{
    public List<FollowUp> FollowUps { get; set; } = new();
    public List<Customer> Customers { get; set; } = new();
    public List<Lead> Leads { get; set; } = new();
    public List<ApplicationUser> Users { get; set; } = new();
    public DateTime? Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AssignedUser { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
