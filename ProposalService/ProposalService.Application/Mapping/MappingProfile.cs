using AutoMapper;
using ProposalService.Application.DTOs;
using ProposalService.Domain.Entities;

namespace ProposalService.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Proposal, ProposalDto>();
    }
}