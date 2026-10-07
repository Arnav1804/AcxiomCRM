using AxciomCRM.Models;

namespace AxciomCRM.ViewModels;

public class OpportunityListViewModel
{
    public List<Opportunity> Opportunities { get; set; } = new();
    public List<Customer> Customers { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string Stage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
