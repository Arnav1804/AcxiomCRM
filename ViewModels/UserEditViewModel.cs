using System.ComponentModel.DataAnnotations;

namespace AxciomCRM.ViewModels;

public class UserEditViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
