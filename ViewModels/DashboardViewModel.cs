namespace AxciomCRM.ViewModels;

public class DashboardViewModel
{
    public int TotalCustomers { get; set; }
    public int TotalLeads { get; set; }
    public int OpenLeads { get; set; }
    public int TotalOpportunities { get; set; }
    public int OpenOpportunities { get; set; }
    public int Open => OpenOpportunities;
    public int WonOpportunities { get; set; }
    public int Won => WonOpportunities;
    public int LostOpportunities { get; set; }
    public int Lost => LostOpportunities;
    public decimal TotalPipelineValue { get; set; }

    public List<string> LeadStatusLabels { get; set; } = new();
    public List<int> LeadStatusCounts { get; set; } = new();

    public List<string> PipelineStageLabels { get; set; } = new();
    public List<int> PipelineStageCounts { get; set; } = new();
    public List<decimal> PipelineStageAmounts { get; set; } = new();

    public List<string> MonthlySalesLabels { get; set; } = new();
    public List<decimal> MonthlySalesAmounts { get; set; } = new();
}
