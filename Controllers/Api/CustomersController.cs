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
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var createdBy = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        var customers = _customerService.GetAllList(createdBy);
        var dtos = new List<CustomerDto>();
        foreach (var c in customers)
        {
            dtos.Add(MapToDto(c));
        }

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var customer = _customerService.GetById(id);
        if (customer == null)
        {
            return NotFound();
        }

        if (User.IsInRole(AppConstants.SalesExecutive) && customer.CreatedBy != GetUserId())
        {
            return Forbid();
        }

        return Ok(MapToDto(customer));
    }

    [HttpPost]
    public IActionResult Create([FromBody] CustomerDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (_customerService.EmailExists(dto.Email))
        {
            return Conflict(new { message = "A customer with this email already exists." });
        }

        if (_customerService.PhoneExists(dto.Phone))
        {
            return Conflict(new { message = "A customer with this phone number already exists." });
        }

        var customer = MapToEntity(dto);
        customer.CreatedBy = GetUserId();
        customer.CreatedDate = DateTime.Now;
        if (string.IsNullOrWhiteSpace(customer.Status))
        {
            customer.Status = "Active";
        }

        _customerService.Create(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, MapToDto(customer));
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] CustomerDto dto)
    {
        var existing = _customerService.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        if (User.IsInRole(AppConstants.SalesExecutive) && existing.CreatedBy != GetUserId())
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (_customerService.EmailExists(dto.Email, id))
        {
            return Conflict(new { message = "A customer with this email already exists." });
        }

        if (_customerService.PhoneExists(dto.Phone, id))
        {
            return Conflict(new { message = "A customer with this phone number already exists." });
        }

        existing.CustomerName = dto.CustomerName;
        existing.Email = dto.Email;
        existing.Phone = dto.Phone;
        existing.CompanyName = dto.CompanyName;
        existing.Address = dto.Address;
        existing.City = dto.City;
        existing.State = dto.State;
        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            existing.Status = dto.Status;
        }

        _customerService.Update(existing);
        return Ok(MapToDto(existing));
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existing = _customerService.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        if (User.IsInRole(AppConstants.SalesExecutive) && existing.CreatedBy != GetUserId())
        {
            return Forbid();
        }

        _customerService.Delete(existing);
        return Ok(MapToDto(existing));
    }

    private CustomerDto MapToDto(Customer c)
    {
        return new CustomerDto
        {
            CustomerId = c.CustomerId,
            CustomerCode = c.CustomerCode,
            CustomerName = c.CustomerName,
            Email = c.Email,
            Phone = c.Phone,
            CompanyName = c.CompanyName,
            Address = c.Address,
            City = c.City,
            State = c.State,
            Status = c.Status,
            CreatedDate = c.CreatedDate,
            CreatedBy = c.CreatedBy
        };
    }

    private Customer MapToEntity(CustomerDto dto)
    {
        return new Customer
        {
            CustomerId = dto.CustomerId,
            CustomerCode = dto.CustomerCode,
            CustomerName = dto.CustomerName,
            Email = dto.Email,
            Phone = dto.Phone,
            CompanyName = dto.CompanyName,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status,
            CreatedDate = dto.CreatedDate,
            CreatedBy = dto.CreatedBy
        };
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
