using AxciomCRM.Data;
using AxciomCRM.Models;
using Microsoft.AspNetCore.Http;

namespace AxciomCRM.Services;

public class AuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Log(string userId, string action, string entityName, string recordId, string? oldValue, string? newValue)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            RecordId = recordId,
            OldValue = oldValue,
            NewValue = newValue,
            CreatedDate = DateTime.Now,
            IpAddress = ipAddress
        };

        _context.AuditLogs.Add(auditLog);
        _context.SaveChanges();
    }
}
