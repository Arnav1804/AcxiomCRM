using AxciomCRM.Models;
using Microsoft.AspNetCore.Identity;

namespace AxciomCRM.Data;

public static class DbSeeder
{
    public static async Task Seed(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        string[] roles = { AppConstants.Admin, AppConstants.Manager, AppConstants.SalesExecutive };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await CreateUser(userManager, "admin@acxiomcrm.com", "Admin User", AppConstants.Admin);
        await CreateUser(userManager, "manager@acxiomcrm.com", "Manager User", AppConstants.Manager);
        await CreateUser(userManager, "sales@acxiomcrm.com", "Sales User", AppConstants.SalesExecutive);

        var salesUser = await userManager.FindByEmailAsync("sales@acxiomcrm.com");
        var adminUser = await userManager.FindByEmailAsync("admin@acxiomcrm.com");

        string salesId = salesUser?.Id ?? string.Empty;
        string adminId = adminUser?.Id ?? string.Empty;

        // seed customers if empty
        if (!context.Customers.Any())
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    CustomerCode = "CUST0001",
                    CustomerName = "Acme Corporation",
                    Email = "contact@acme.com",
                    Phone = "9876543210",
                    CompanyName = "Acme Corp",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddDays(-60),
                    CreatedBy = salesId
                },
                new Customer
                {
                    CustomerCode = "CUST0002",
                    CustomerName = "Global Logistics",
                    Email = "info@globallogistics.com",
                    Phone = "9876543211",
                    CompanyName = "Global Logistics Ltd",
                    City = "Delhi",
                    State = "Delhi",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddDays(-50),
                    CreatedBy = salesId
                },
                new Customer
                {
                    CustomerCode = "CUST0003",
                    CustomerName = "Apex Solutions",
                    Email = "support@apexsolutions.com",
                    Phone = "9876543212",
                    CompanyName = "Apex Solutions Inc",
                    City = "Bengaluru",
                    State = "Karnataka",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddDays(-40),
                    CreatedBy = salesId
                },
                new Customer
                {
                    CustomerCode = "CUST0004",
                    CustomerName = "Starlight Media",
                    Email = "hello@starlightmedia.com",
                    Phone = "9876543213",
                    CompanyName = "Starlight Media Group",
                    City = "Pune",
                    State = "Maharashtra",
                    Status = "Active",
                    CreatedDate = DateTime.Now.AddDays(-30),
                    CreatedBy = adminId
                }
            };
            context.Customers.AddRange(customers);
            context.SaveChanges();
        }

        // seed leads if empty
        if (!context.Leads.Any())
        {
            var leads = new List<Lead>
            {
                new Lead
                {
                    LeadCode = "LEAD0001",
                    LeadName = "Amit Sharma",
                    Email = "amit.sharma@techlead.com",
                    Phone = "9123456781",
                    CompanyName = "TechLead Solutions",
                    Source = "Website",
                    Status = "New",
                    ExpectedValue = 15000,
                    CreatedDate = DateTime.Now.AddDays(-10),
                    AssignedTo = salesId
                },
                new Lead
                {
                    LeadCode = "LEAD0002",
                    LeadName = "Pooja Patel",
                    Email = "pooja.patel@innovatech.com",
                    Phone = "9123456782",
                    CompanyName = "InnovaTech",
                    Source = "Referral",
                    Status = "New",
                    ExpectedValue = 20000,
                    CreatedDate = DateTime.Now.AddDays(-8),
                    AssignedTo = salesId
                },
                new Lead
                {
                    LeadCode = "LEAD0003",
                    LeadName = "Rahul Verma",
                    Email = "rahul.verma@apexcloud.com",
                    Phone = "9123456783",
                    CompanyName = "Apex Cloud",
                    Source = "Cold Call",
                    Status = "Contacted",
                    ExpectedValue = 35000,
                    CreatedDate = DateTime.Now.AddDays(-15),
                    AssignedTo = salesId
                },
                new Lead
                {
                    LeadCode = "LEAD0004",
                    LeadName = "Sneha Rao",
                    Email = "sneha.rao@raosoftware.com",
                    Phone = "9123456784",
                    CompanyName = "Rao Software",
                    Source = "Website",
                    Status = "Contacted",
                    ExpectedValue = 25000,
                    CreatedDate = DateTime.Now.AddDays(-12),
                    AssignedTo = salesId
                },
                new Lead
                {
                    LeadCode = "LEAD0005",
                    LeadName = "Vikram Singh",
                    Email = "vikram.singh@singhenterprises.com",
                    Phone = "9123456785",
                    CompanyName = "Singh Enterprises",
                    Source = "Trade Show",
                    Status = "Qualified",
                    ExpectedValue = 50000,
                    CreatedDate = DateTime.Now.AddDays(-20),
                    AssignedTo = salesId
                },
                new Lead
                {
                    LeadCode = "LEAD0006",
                    LeadName = "Kavita Reddy",
                    Email = "kavita.reddy@reddyinfra.com",
                    Phone = "9123456786",
                    CompanyName = "Reddy Infra",
                    Source = "Partner",
                    Status = "Qualified",
                    ExpectedValue = 45000,
                    CreatedDate = DateTime.Now.AddDays(-18),
                    AssignedTo = salesId
                },
                new Lead
                {
                    LeadCode = "LEAD0007",
                    LeadName = "Deepak Kumar",
                    Email = "deepak.kumar@kumartraders.com",
                    Phone = "9123456787",
                    CompanyName = "Kumar Traders",
                    Source = "Cold Call",
                    Status = "Unqualified",
                    ExpectedValue = 10000,
                    CreatedDate = DateTime.Now.AddDays(-25),
                    AssignedTo = salesId
                },
                new Lead
                {
                    LeadCode = "LEAD0008",
                    LeadName = "Ritu Gupta",
                    Email = "ritu.gupta@guptadesigns.com",
                    Phone = "9123456788",
                    CompanyName = "Gupta Designs",
                    Source = "Website",
                    Status = "Converted",
                    ExpectedValue = 30000,
                    CreatedDate = DateTime.Now.AddDays(-30),
                    AssignedTo = salesId
                },
                new Lead
                {
                    LeadCode = "LEAD0009",
                    LeadName = "Manoj Joshi",
                    Email = "manoj.joshi@joshiventures.com",
                    Phone = "9123456789",
                    CompanyName = "Joshi Ventures",
                    Source = "Referral",
                    Status = "Lost",
                    ExpectedValue = 20000,
                    CreatedDate = DateTime.Now.AddDays(-28),
                    AssignedTo = salesId
                }
            };
            context.Leads.AddRange(leads);
            context.SaveChanges();
        }

        // seed opportunities if empty
        if (!context.Opportunities.Any())
        {
            var firstCust = context.Customers.FirstOrDefault();
            int custId = firstCust != null ? firstCust.CustomerId : 1;

            var opportunities = new List<Opportunity>();

            // open opportunities across stages
            opportunities.Add(new Opportunity
            {
                OpportunityName = "Cloud Infrastructure Upgrade",
                CustomerId = custId,
                Amount = 30000,
                Stage = "Qualification",
                Probability = 20,
                ExpectedCloseDate = DateTime.Today.AddDays(25),
                Status = "Open",
                CreatedDate = DateTime.Now.AddDays(-15),
                AssignedTo = salesId
            });

            opportunities.Add(new Opportunity
            {
                OpportunityName = "ERP Integration Service",
                CustomerId = custId,
                Amount = 45000,
                Stage = "Proposal",
                Probability = 50,
                ExpectedCloseDate = DateTime.Today.AddDays(35),
                Status = "Open",
                CreatedDate = DateTime.Now.AddDays(-20),
                AssignedTo = salesId
            });

            opportunities.Add(new Opportunity
            {
                OpportunityName = "Annual Support Contract",
                CustomerId = custId,
                Amount = 60000,
                Stage = "Negotiation",
                Probability = 80,
                ExpectedCloseDate = DateTime.Today.AddDays(15),
                Status = "Open",
                CreatedDate = DateTime.Now.AddDays(-10),
                AssignedTo = salesId
            });

            // lost opportunity
            opportunities.Add(new Opportunity
            {
                OpportunityName = "Legacy Migration Pitch",
                CustomerId = custId,
                Amount = 15000,
                Stage = "Lost",
                Probability = 0,
                ExpectedCloseDate = DateTime.Today.AddDays(-40),
                Status = "Lost",
                CreatedDate = DateTime.Now.AddDays(-50),
                AssignedTo = salesId
            });

            // won opportunities for monthly sales (last 6 months)
            decimal[] monthlyAmounts = { 18000, 25000, 22000, 32000, 40000, 48000 };
            for (int i = 5; i >= 0; i--)
            {
                var targetMonth = DateTime.Today.AddMonths(-i);
                var closeDate = new DateTime(targetMonth.Year, targetMonth.Month, Math.Min(15, DateTime.DaysInMonth(targetMonth.Year, targetMonth.Month)));
                var createdDate = new DateTime(targetMonth.Year, targetMonth.Month, 2);

                opportunities.Add(new Opportunity
                {
                    OpportunityName = "Won Deal - " + targetMonth.ToString("MMM yyyy"),
                    CustomerId = custId,
                    Amount = monthlyAmounts[5 - i],
                    Stage = "Won",
                    Probability = 100,
                    ExpectedCloseDate = closeDate,
                    Status = "Won",
                    CreatedDate = createdDate,
                    AssignedTo = salesId
                });
            }

            context.Opportunities.AddRange(opportunities);
            context.SaveChanges();
        }

        // seed follow-ups if empty
        if (!context.FollowUps.Any())
        {
            var firstCust = context.Customers.FirstOrDefault();
            var firstLead = context.Leads.FirstOrDefault();

            var followUps = new List<FollowUp>
            {
                new FollowUp
                {
                    CustomerId = firstCust?.CustomerId,
                    FollowUpDate = DateTime.Today.AddDays(2),
                    FollowUpType = "Call",
                    Remarks = "Follow up on software proposal",
                    Status = "Pending",
                    AssignedTo = salesId
                },
                new FollowUp
                {
                    LeadId = firstLead?.LeadId,
                    FollowUpDate = DateTime.Today.AddDays(1),
                    FollowUpType = "Meeting",
                    Remarks = "Demo session with client team",
                    Status = "Pending",
                    AssignedTo = salesId
                },
                new FollowUp
                {
                    CustomerId = firstCust?.CustomerId,
                    FollowUpDate = DateTime.Today.AddDays(-3),
                    FollowUpType = "Email",
                    Remarks = "Sent updated pricing sheet",
                    Status = "Completed",
                    AssignedTo = salesId
                }
            };
            context.FollowUps.AddRange(followUps);
            context.SaveChanges();
        }
    }

    private static async Task CreateUser(UserManager<ApplicationUser> userManager, string email, string fullName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var result = await userManager.CreateAsync(user, "Acxiom@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
