using System.ComponentModel.DataAnnotations;

namespace AxciomCRM.Models;

public class Opportunity
{
    public int OpportunityId { get; set; }

    [Required]
    [StringLength(100)]
    public string OpportunityName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a customer.")]
    public int? CustomerId { get; set; }

    public int? LeadId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Opportunity Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(20)]
    public string Stage { get; set; } = "Qualification";

    [Range(0, 100, ErrorMessage = "Probability must be between 0 and 100.")]
    public int Probability { get; set; }

    [Required]
    public DateTime ExpectedCloseDate { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Open";

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Required]
    public string AssignedTo { get; set; } = string.Empty;
}
