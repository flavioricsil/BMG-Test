using Microsoft.AspNetCore.Mvc;
using ContractService.Application.DTOs;
using ContractService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContractService.API.Controllers;

[ApiController]
[Route("api/contracts")]
public class ContractController : ControllerBase
{
    private readonly IContractService _service;

    public ContractController(IContractService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult CreateContract([FromBody] CreateContractRequestDTO request)
    {
        var result = _service.CreateContract(request);
        return CreatedAtAction(nameof(CreateContract), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var contract = await _service.GetByIdAsync(id);
        return contract is null ? NotFound() : Ok(contract);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var contracts = await _service.GetAllAsync();
        return Ok(contracts);
    }
}
