using AutoMapper;
using ContractService.Application.DTOs;
using ContractService.Domain.Entities;

namespace ContractService.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Contract, ContractDTO>();
    }
}