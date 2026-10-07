using System.ComponentModel.DataAnnotations;

namespace AxciomCRM.DTOs;

public class LeadDto
{
    public int LeadId { get; set; }

    public string LeadCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LeadName { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^[6-9][0-9]{9}$", ErrorMessage = "Phone must be 10 digits and start with 6 to 9.")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Source { get; set; } = string.Empty;

    [StringLength(20)]
    public string Status { get; set; } = "New";

    [Range(0, 100000000, ErrorMessage = "Expected value must be between 0 and 100000000.")]
    public decimal ExpectedValue { get; set; }

    public DateTime CreatedDate { get; set; }

    public string AssignedTo { get; set; } = string.Empty;
}
