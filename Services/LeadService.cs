using AxciomCRM.Data;
using AxciomCRM.Models;
using AxciomCRM.ViewModels;

namespace AxciomCRM.Services;

public class LeadService
{
    private readonly ApplicationDbContext _context;

    public LeadService(ApplicationDbContext context)
    {
        _context = context;
    }

    public LeadListViewModel GetAll(string? name, string? company, string? status, string? assignedUser, int page, string? salesUserId = null)
    {
        var leads = _context.Leads.ToList();

        if (!string.IsNullOrWhiteSpace(name))
        {
            leads = leads.Where(l => l.LeadName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(company))
        {
            leads = leads.Where(l => l.CompanyName.Contains(company, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            leads = leads.Where(l => l.Status == status).ToList();
        }

        if (!string.IsNullOrWhiteSpace(assignedUser))
        {
            leads = leads.Where(l => l.AssignedTo == assignedUser).ToList();
        }

        if (!string.IsNullOrEmpty(salesUserId))
        {
            leads = leads.Where(l => l.AssignedTo == salesUserId).ToList();
        }

        leads = leads.OrderByDescending(l => l.CreatedDate).ToList();
        var totalPages = (int)Math.Ceiling(leads.Count / 10.0);
        if (page < 1)
        {
            page = 1;
        }

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        return new LeadListViewModel
        {
            Leads = leads.Skip((page - 1) * 10).Take(10).ToList(),
            Name = name ?? string.Empty,
            Company = company ?? string.Empty,
            Status = status ?? string.Empty,
            AssignedUser = assignedUser ?? string.Empty,
            Page = page,
            TotalPages = totalPages
        };
    }

    public List<Lead> GetAllList(string? salesUserId = null)
    {
        var leads = _context.Leads.ToList();
        if (!string.IsNullOrEmpty(salesUserId))
        {
            leads = leads.Where(l => l.AssignedTo == salesUserId).ToList();
        }

        return leads.OrderByDescending(l => l.CreatedDate).ToList();
    }

    public Lead? GetById(int id)
    {
        return _context.Leads.FirstOrDefault(l => l.LeadId == id);
    }

    public void Create(Lead lead)
    {
        lead.LeadCode = string.Empty;
        _context.Leads.Add(lead);
        _context.SaveChanges();

        lead.LeadCode = "LEAD" + lead.LeadId.ToString("D4");
        _context.SaveChanges();
    }

    public void Update(Lead lead)
    {
        _context.Leads.Update(lead);
        _context.SaveChanges();
    }

    public void Delete(Lead lead)
    {
        _context.Leads.Remove(lead);
        _context.SaveChanges();
    }

    public void Convert(Lead lead)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Email == lead.Email);
        if (customer == null)
        {
            customer = new Customer
            {
                CustomerCode = string.Empty,
                CustomerName = lead.LeadName,
                Email = lead.Email,
                Phone = lead.Phone,
                CompanyName = lead.CompanyName,
                City = "Not Provided",
                State = "Not Provided",
                Status = "Active",
                CreatedDate = DateTime.Now,
                CreatedBy = lead.AssignedTo
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();
            customer.CustomerCode = "CUST" + customer.CustomerId.ToString("D4");
            _context.SaveChanges();
        }

        var opportunity = new Opportunity
        {
            OpportunityName = lead.LeadName + " Opportunity",
            CustomerId = customer.CustomerId,
            LeadId = lead.LeadId,
            Amount = lead.ExpectedValue,
            Stage = "Qualification",
            Probability = 0,
            ExpectedCloseDate = DateTime.Today.AddDays(30),
            Status = "Open",
            CreatedDate = DateTime.Now,
            AssignedTo = lead.AssignedTo
        };

        _context.Opportunities.Add(opportunity);
        lead.Status = "Converted";
        _context.SaveChanges();
    }
}
