using System.ComponentModel.DataAnnotations;

namespace AxciomCRM.Models;

public class FollowUp
{
    public int FollowUpId { get; set; }

    public int? CustomerId { get; set; }

    public int? LeadId { get; set; }

    [Required]
    public DateTime FollowUpDate { get; set; }

    [Required]
    [StringLength(50)]
    public string FollowUpType { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Remarks { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending";

    [Required]
    public string AssignedTo { get; set; } = string.Empty;
}
