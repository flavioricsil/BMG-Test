using ProposalService.Domain.Enums;

namespace ProposalService.Application.DTOs;

public class ProposalDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string InsuranceType { get; set; } = string.Empty;
    public ProposalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}