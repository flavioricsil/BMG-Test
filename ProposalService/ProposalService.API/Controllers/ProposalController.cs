using Microsoft.AspNetCore.Mvc;
using ProposalService.Application.DTOs;
using ProposalService.Application.Interfaces;

namespace ProposalService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProposalController : ControllerBase
{
    private readonly IProposalService _service;

    public ProposalController(IProposalService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProposal([FromBody] CreateProposalRequestDto request)
    {
        var id = await _service.CreateProposalAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var proposal = await _service.GetByIdAsync(id);
        return proposal is null ? NotFound() : Ok(proposal);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var proposals = await _service.GetAllAsync();
        return Ok(proposals);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] string status)
    {
        var success = await _service.ChangeStatusAsync(id, status);
        return success ? NoContent() : BadRequest("Invalid status or proposal not found.");
    }
}