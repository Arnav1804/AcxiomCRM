using AxciomCRM.Data;
using AxciomCRM.Models;
using AxciomCRM.ViewModels;

namespace AxciomCRM.Services;

public class CustomerService
{
    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public CustomerListViewModel GetAll(string? search, int page, string? createdBy = null)
    {
        var customers = _context.Customers.ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            customers = customers.Where(c => c.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase)
                || c.Email.Contains(search, StringComparison.OrdinalIgnoreCase)
                || c.Phone.Contains(search, StringComparison.OrdinalIgnoreCase)
                || c.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(createdBy))
        {
            customers = customers.Where(c => c.CreatedBy == createdBy).ToList();
        }

        customers = customers.OrderByDescending(c => c.CreatedDate).ToList();
        var totalPages = (int)Math.Ceiling(customers.Count / 10.0);
        if (page < 1)
        {
            page = 1;
        }

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        return new CustomerListViewModel
        {
            Customers = customers.Skip((page - 1) * 10).Take(10).ToList(),
            Search = search ?? string.Empty,
            Page = page,
            TotalPages = totalPages
        };
    }

    public List<Customer> GetAllList(string? createdBy = null)
    {
        var customers = _context.Customers.ToList();
        if (!string.IsNullOrEmpty(createdBy))
        {
            customers = customers.Where(c => c.CreatedBy == createdBy).ToList();
        }

        return customers.OrderByDescending(c => c.CreatedDate).ToList();
    }

    public Customer? GetById(int id)
    {
        return _context.Customers.FirstOrDefault(c => c.CustomerId == id);
    }

    public void Create(Customer customer)
    {
        customer.CustomerCode = string.Empty;
        _context.Customers.Add(customer);
        _context.SaveChanges();

        customer.CustomerCode = "CUST" + customer.CustomerId.ToString("D4");
        _context.SaveChanges();
    }

    public void Update(Customer customer)
    {
        _context.Customers.Update(customer);
        _context.SaveChanges();
    }

    public void Delete(Customer customer)
    {
        customer.Status = "Inactive";
        _context.SaveChanges();
    }

    public bool EmailExists(string email, int? customerId = null)
    {
        return _context.Customers.Any(c => c.Email == email && (!customerId.HasValue || c.CustomerId != customerId.Value));
    }

    public bool PhoneExists(string phone, int? customerId = null)
    {
        return _context.Customers.Any(c => c.Phone == phone && (!customerId.HasValue || c.CustomerId != customerId.Value));
    }
}
