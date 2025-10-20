using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProposalService.Application.Events;

public class ProposalApprovedEvent
{
    public Guid ProposalId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string InsuranceType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}


