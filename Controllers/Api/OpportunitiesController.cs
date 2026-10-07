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
public class OpportunitiesController : ControllerBase
{
    private readonly OpportunityService _opportunityService;
    private readonly CustomerService _customerService;

    public OpportunitiesController(OpportunityService opportunityService, CustomerService customerService)
    {
        _opportunityService = opportunityService;
        _customerService = customerService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var assignedTo = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var opportunities = _opportunityService.GetAllList(assignedTo);
        var dtos = new List<OpportunityDto>();
        foreach (var o in opportunities)
        {
            dtos.Add(MapToDto(o));
        }

        return Ok(dtos);
    }

    [HttpPost]
    public IActionResult Create([FromBody] OpportunityDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!dto.CustomerId.HasValue)
        {
            return BadRequest(new { message = "CustomerId is required." });
        }

        var customer = _customerService.GetById(dto.CustomerId.Value);
        if (customer == null)
        {
            return BadRequest(new { message = "Customer not found." });
        }

        if (User.IsInRole(AppConstants.SalesExecutive) && customer.CreatedBy != GetUserId())
        {
            return Forbid();
        }

        if (dto.Amount <= 0)
        {
            return BadRequest(new { message = "Opportunity Amount must be greater than 0." });
        }

        if (string.IsNullOrWhiteSpace(dto.Stage) || !AppConstants.OpportunityStages.Contains(dto.Stage))
        {
            return BadRequest(new { message = "Invalid opportunity stage." });
        }

        if (dto.Probability < 0 || dto.Probability > 100)
        {
            return BadRequest(new { message = "Probability must be between 0 and 100." });
        }

        string status;
        if (dto.Stage == "Won")
        {
            status = "Won";
        }
        else if (dto.Stage == "Lost")
        {
            status = "Lost";
        }
        else
        {
            status = "Open";
        }

        if (status == "Open" && dto.ExpectedCloseDate.Date < DateTime.Today)
        {
            return BadRequest(new { message = "Expected Close Date cannot be in the past." });
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

        var opportunity = MapToEntity(dto);
        opportunity.AssignedTo = assignedTo;
        opportunity.CreatedDate = DateTime.Now;
        opportunity.Status = status;

        _opportunityService.Create(opportunity);
        return StatusCode(StatusCodes.Status201Created, MapToDto(opportunity));
    }

    private OpportunityDto MapToDto(Opportunity o)
    {
        return new OpportunityDto
        {
            OpportunityId = o.OpportunityId,
            OpportunityName = o.OpportunityName,
            CustomerId = o.CustomerId,
            LeadId = o.LeadId,
            Amount = o.Amount,
            Stage = o.Stage,
            Probability = o.Probability,
            ExpectedCloseDate = o.ExpectedCloseDate,
            Status = o.Status,
            CreatedDate = o.CreatedDate,
            AssignedTo = o.AssignedTo
        };
    }

    private Opportunity MapToEntity(OpportunityDto dto)
    {
        return new Opportunity
        {
            OpportunityId = dto.OpportunityId,
            OpportunityName = dto.OpportunityName,
            CustomerId = dto.CustomerId,
            LeadId = dto.LeadId,
            Amount = dto.Amount,
            Stage = string.IsNullOrWhiteSpace(dto.Stage) ? "Qualification" : dto.Stage,
            Probability = dto.Probability,
            ExpectedCloseDate = dto.ExpectedCloseDate,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Open" : dto.Status,
            CreatedDate = dto.CreatedDate,
            AssignedTo = dto.AssignedTo
        };
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
