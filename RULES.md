Project: AcxiomCRM, a CRM for a placement assessment. ASP.NET Core MVC on .NET 8 (use the installed SDK, tell me in one line if it isn't 8), EF Core with SQLite (template default, app.db), ASP.NET Core Identity, Bootstrap 5, Chart.js (CDN). Namespace AcxiomCRM.

How to work:
- You write the files and run the commands yourself. Never paste code into the chat.
- After each session run `dotnet build` and fix errors yourself (max 3 attempts, then stop and show the exact error).
- Don't ask me questions. Pick the simplest option and move on.
- Don't re-read files you just wrote. Don't add tests, docs, extra features or files not asked for.
- Final reply per session: one or two lines only: "Session N done. Build OK. <anything I must know>". Then append the same one line to PROGRESS.md.

Code style (must look hand-written by a student):
- Simple code: plain if/else, foreach, basic LINQ (Where, FirstOrDefault, ToList, Count, Sum). No generics, repository pattern, AutoMapper, MediatR, interfaces for everything, or clever one-liners.
- Almost no comments. Where needed, short lowercase ones like "// check duplicate email". No XML doc comments, no banners, no #regions.
- Plain default Bootstrap only. No custom CSS theme, gradients, icon libraries or animations.
- Normal names: CustomerService, GetAll, GetById, Create, Update, Delete. Not Handler, Manager, Factory, Provider.
- Small files. ViewModels only where a model alone isn't enough.

Fixed names:
- Roles: Admin, Manager, SalesExecutive
- User: ApplicationUser (FullName, IsActive, CreatedDate)
- Models: Customer, Lead, Opportunity, FollowUp, Activity, AuditLog
- Folders: Models, Data, Services, ViewModels, DTOs, Controllers, Controllers/Api, Views
- Services: AuditService, CustomerService, LeadService, OpportunityService, DashboardService (register in Program.cs)
- Phone: 10 digits starting 6-9. Lead statuses: New, Contacted, Qualified, Unqualified, Converted, Lost. Stages: Qualification, Proposal, Negotiation, Won, Lost.
- Every POST action has [ValidateAntiForgeryToken] and checks ModelState. Validation must work on client (jQuery unobtrusive) and server. Sales executives see only records assigned to them (customers: ones they created). Admin and Manager see everything.
- Lists use a search box and simple paging (10 per page). Delete for customers means Status = Inactive.
