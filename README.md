# AcxiomCRM

A Customer Relationship Management (CRM) application built with ASP.NET Core MVC on .NET 8, EF Core with SQLite, and ASP.NET Core Identity.

## How to Run

1. Open a terminal in the project directory.
2. Run the project:
   ```bash
   dotnet run
   ```
3. Navigate in your browser to the listening URL (e.g. `http://localhost:5127`).
4. The database is initialized and sample seed data is inserted automatically on first startup.

## Seeded Logins

Password for all accounts: `Acxiom@123`

- **Admin**: `admin@acxiomcrm.com`
- **Manager**: `manager@acxiomcrm.com`
- **Sales Executive**: `sales@acxiomcrm.com`

## Features

- **Authentication & Authorization**: Identity-based login with roles (Admin, Manager, SalesExecutive), password policies, lockout, and 401 responses for unauthenticated API calls.
- **Role Scoping**: Sales executives view only their own records; Admin and Manager view all records; unauthorized access attempts to admin pages redirect to Access Denied.
- **Customers**: Customer directory with unique email and phone constraints (10 digits starting with 6-9), search, pagination (10 per page), and soft delete (Status = Inactive).
- **Leads**: Lead tracking with status lifecycle (New, Contacted, Qualified, Unqualified, Converted, Lost), user assignment, search filters, and lead-to-customer conversion.
- **Opportunities & Pipeline**: Opportunity management by stage, probability, and close date, with a dedicated visual Sales Pipeline page.
- **Follow-ups & Activities**: Schedule calls, meetings, emails, and tasks with future date validation and completion tracking.
- **Audit Logging**: Logs successful logins, failed attempts, account lockouts, user edits, and all entity creation, updates, and deletes.
- **Dashboard**: Bootstrap summary cards and 3 Chart.js charts (Lead Status bar chart, Opportunity Pipeline by stage doughnut chart, and Monthly Sales line chart).
- **REST APIs**: `[ApiController]` endpoints for `/api/customers`, `/api/leads`, and `/api/opportunities` with hand-mapped DTOs, role scoping, and standard HTTP status codes (200, 201, 400, 401, 403, 404, 409).
