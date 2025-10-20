using ContractService.Application.DTOs;

namespace ContractService.Application.Interfaces;

public interface IContractService
{
    ContractDTO CreateContract(CreateContractRequestDTO request);
    Task<ContractDTO?> GetByIdAsync(Guid id);
    Task<IEnumerable<ContractDTO>> GetAllAsync();
}
