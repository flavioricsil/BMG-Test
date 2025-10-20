using AutoMapper;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using ProposalService.Application.DTOs;
using ProposalService.Application.Mapping;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Domain.Interfaces;

namespace ProposalService.Tests;

public class ProposalServiceTests
{
    private readonly Mock<IProposalRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ILogger<ProposalService.Application.Services.ProposalService>> _loggerMock;
    private readonly ProposalService.Application.Services.ProposalService _service;

    public ProposalServiceTests()
    {
        _repositoryMock = new Mock<IProposalRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>(); // ⬅️ Mock do terceiro parâmetro

        var loggerFactory = new Microsoft.Extensions.Logging.LoggerFactory();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        }, loggerFactory);
        _mapper = config.CreateMapper();
        _loggerMock = new Mock<ILogger<ProposalService.Application.Services.ProposalService>>();
        _service = new ProposalService.Application.Services.ProposalService(
            _repositoryMock.Object,
            _mapper,
            _publishEndpointMock.Object,
            _loggerMock.Object// ⬅️ Passando o mock corretamente
        );
    }

    [Fact]
    public async Task CreateProposalAsync_ShouldReturnNewId()
    {
        // Arrange
        var request = new CreateProposalRequestDto
        {
            CustomerName = "João Silva",
            InsuranceType = "Auto"
        };

        // Act
        var result = await _service.CreateProposalAsync(request);

        // Assert
        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Proposal>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedDto()
    {
        // Arrange
        var proposal = new Proposal("Maria", "Vida");
        _repositoryMock.Setup(r => r.GetByIdAsync(proposal.Id)).ReturnsAsync(proposal);

        // Act
        var result = await _service.GetByIdAsync(proposal.Id);

        // Assert
        result.Should().NotBeNull();
        result!.CustomerName.Should().Be("Maria");
        result.InsuranceType.Should().Be("Vida");
    }

    [Fact]
    public async Task ChangeStatusAsync_ShouldUpdateStatus_WhenValid()
    {
        // Arrange
        var proposal = new Proposal("Carlos", "Saúde");
        _repositoryMock.Setup(r => r.GetByIdAsync(proposal.Id)).ReturnsAsync(proposal);

        // Act
        var result = await _service.ChangeStatusAsync(proposal.Id, "Approved");

        // Assert
        result.Should().BeTrue();
        proposal.Status.Should().Be(ProposalStatus.Approved);
        _repositoryMock.Verify(r => r.UpdateAsync(proposal), Times.Once);
    }

    [Fact]
    public async Task ChangeStatusAsync_ShouldReturnFalse_WhenInvalidStatus()
    {
        // Arrange
        var proposal = new Proposal("Ana", "Residencial");
        _repositoryMock.Setup(r => r.GetByIdAsync(proposal.Id)).ReturnsAsync(proposal);

        // Act
        var result = await _service.ChangeStatusAsync(proposal.Id, "InvalidStatus");

        // Assert
        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Proposal>()), Times.Never);
    }
}