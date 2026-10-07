using AxciomCRM.Models;
using AxciomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AxciomCRM.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly CustomerService _customerService;
    private readonly AuditService _auditService;

    public CustomersController(CustomerService customerService, AuditService auditService)
    {
        _customerService = customerService;
        _auditService = auditService;
    }

    public IActionResult Index(string? search, int page = 1)
    {
        var createdBy = User.IsInRole(AppConstants.SalesExecutive) ? GetUserId() : null;
        return View(_customerService.GetAll(search, page, createdBy));
    }

    public IActionResult Details(int id)
    {
        var customer = GetCustomer(id);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    public IActionResult Create()
    {
        return View(new Customer());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Customer customer)
    {
        ModelState.Remove("CustomerCode");
        ModelState.Remove("CreatedBy");
        customer.CreatedBy = GetUserId();
        customer.CreatedDate = DateTime.Now;
        CheckDuplicate(customer);

        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        _customerService.Create(customer);
        _auditService.Log(GetUserId(), "Create", "Customer", customer.CustomerId.ToString(), null, Describe(customer));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var customer = GetCustomer(id);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Customer customer)
    {
        ModelState.Remove("CustomerCode");
        ModelState.Remove("CreatedBy");
        var existingCustomer = GetCustomer(id);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        customer.CustomerId = id;
        customer.CustomerCode = existingCustomer.CustomerCode;
        customer.CreatedBy = existingCustomer.CreatedBy;
        customer.CreatedDate = existingCustomer.CreatedDate;
        var oldValue = Describe(existingCustomer);
        CheckDuplicate(customer);

        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        _customerService.Update(customer);
        _auditService.Log(GetUserId(), "Update", "Customer", customer.CustomerId.ToString(), oldValue, Describe(customer));
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var customer = GetCustomer(id);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var customer = GetCustomer(id);
        if (customer == null)
        {
            return NotFound();
        }

        var oldValue = Describe(customer);
        _customerService.Delete(customer);
        _auditService.Log(GetUserId(), "Delete", "Customer", customer.CustomerId.ToString(), oldValue, Describe(customer));
        return RedirectToAction(nameof(Index));
    }

    private Customer? GetCustomer(int id)
    {
        var customer = _customerService.GetById(id);
        if (customer != null && User.IsInRole(AppConstants.SalesExecutive) && customer.CreatedBy != GetUserId())
        {
            return null;
        }

        return customer;
    }

    private void CheckDuplicate(Customer customer)
    {
        if (_customerService.EmailExists(customer.Email, customer.CustomerId == 0 ? null : customer.CustomerId))
        {
            ModelState.AddModelError("Email", "A customer with this email already exists.");
        }

        if (_customerService.PhoneExists(customer.Phone, customer.CustomerId == 0 ? null : customer.CustomerId))
        {
            ModelState.AddModelError("Phone", "A customer with this phone number already exists.");
        }
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    private string Describe(Customer customer)
    {
        return customer.CustomerName + "; " + customer.Email + "; " + customer.Status;
    }
}
