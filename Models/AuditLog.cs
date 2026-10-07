using System.ComponentModel.DataAnnotations;

namespace AxciomCRM.Models;

public class AuditLog
{
    public int AuditLogId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EntityName { get; set; } = string.Empty;

    [Required]
    public string RecordId { get; set; } = string.Empty;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [StringLength(50)]
    public string? IpAddress { get; set; }
}
