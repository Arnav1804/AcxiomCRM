using System.Security.Claims;
using AxciomCRM.DTOs;
using AxciomCRM.Models;
using AxciomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxciomCRM.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly LeadService _leadService;

    public LeadsController(LeadService leadService)
    {
        _leadService = leadService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var salesUserId = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var leads = _leadService.GetAllList(salesUserId);
        var dtos = new List<LeadDto>();
        foreach (var l in leads)
        {
            dtos.Add(MapToDto(l));
        }

        return Ok(dtos);
    }

    [HttpPost]
    public IActionResult Create([FromBody] LeadDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!string.IsNullOrWhiteSpace(dto.Status) && !AppConstants.LeadStatuses.Contains(dto.Status))
        {
            return BadRequest(new { message = "Invalid lead status." });
        }

        if (dto.ExpectedValue < 0)
        {
            return BadRequest(new { message = "Expected value cannot be negative." });
        }

        string assignedTo;
        if (User.IsInRole(AppConstants.SalesExecutive))
        {
            assignedTo = GetUserId();
        }
        else
        {
            assignedTo = string.IsNullOrWhiteSpace(dto.AssignedTo) ? GetUserId() : dto.AssignedTo;
        }

        var lead = MapToEntity(dto);
        lead.AssignedTo = assignedTo;
        lead.CreatedDate = DateTime.Now;
        if (string.IsNullOrWhiteSpace(lead.Status))
        {
            lead.Status = "New";
        }

        _leadService.Create(lead);
        return StatusCode(StatusCodes.Status201Created, MapToDto(lead));
    }

    private LeadDto MapToDto(Lead l)
    {
        return new LeadDto
        {
            LeadId = l.LeadId,
            LeadCode = l.LeadCode,
            LeadName = l.LeadName,
            Email = l.Email,
            Phone = l.Phone,
            CompanyName = l.CompanyName,
            Source = l.Source,
            Status = l.Status,
            ExpectedValue = l.ExpectedValue,
            CreatedDate = l.CreatedDate,
            AssignedTo = l.AssignedTo
        };
    }

    private Lead MapToEntity(LeadDto dto)
    {
        return new Lead
        {
            LeadId = dto.LeadId,
            LeadCode = dto.LeadCode,
            LeadName = dto.LeadName,
            Email = dto.Email,
            Phone = dto.Phone,
            CompanyName = dto.CompanyName,
            Source = dto.Source,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "New" : dto.Status,
            ExpectedValue = dto.ExpectedValue,
            CreatedDate = dto.CreatedDate,
            AssignedTo = dto.AssignedTo
        };
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
