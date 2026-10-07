using System.ComponentModel.DataAnnotations;

namespace AxciomCRM.Models;

public class Activity
{
    public int ActivityId { get; set; }

    [Required]
    [StringLength(50)]
    public string ActivityType { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Subject { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime ActivityDate { get; set; }

    public int? CustomerId { get; set; }

    public int? LeadId { get; set; }

    [Required]
    public string AssignedTo { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Open";
}
