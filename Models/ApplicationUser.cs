using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AxciomCRM.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
