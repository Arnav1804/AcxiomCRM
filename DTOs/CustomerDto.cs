using System.ComponentModel.DataAnnotations;

namespace AxciomCRM.DTOs;

public class CustomerDto
{
    public int CustomerId { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

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

    [StringLength(250)]
    public string? Address { get; set; }

    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string State { get; set; } = string.Empty;

    public string Status { get; set; } = "Active";

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}
