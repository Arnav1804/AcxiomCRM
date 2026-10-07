using AxciomCRM.Models;

namespace AxciomCRM.ViewModels;

public class CustomerListViewModel
{
    public List<Customer> Customers { get; set; } = new();
    public string Search { get; set; } = string.Empty;
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
