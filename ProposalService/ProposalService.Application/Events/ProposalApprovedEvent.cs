namespace ProposalService.Application.Events;

public class ProposalApprovedEvent
{
    public Guid ProposalId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string InsuranceType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
