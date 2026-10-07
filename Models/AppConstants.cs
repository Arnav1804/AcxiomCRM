namespace AxciomCRM.Models;

public static class AppConstants
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string SalesExecutive = "SalesExecutive";

    public static readonly string[] LeadStatuses = { "New", "Contacted", "Qualified", "Unqualified", "Converted", "Lost" };
    public static readonly string[] OpportunityStages = { "Qualification", "Proposal", "Negotiation", "Won", "Lost" };
    public static readonly string[] CustomerStatuses = { "Active", "Inactive" };
    public static readonly string[] FollowUpStatuses = { "Planned", "Completed", "Missed", "Cancelled" };
    public static readonly string[] ActivityTypes = { "Call", "Meeting", "Email", "Task" };
    public static readonly string[] ActivityStatuses = { "Open", "Completed" };
}
