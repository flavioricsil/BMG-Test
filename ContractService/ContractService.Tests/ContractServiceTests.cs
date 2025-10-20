using ContractService.Application.DTOs;
using ContractService.Application.Interfaces;
using ContractService.Domain.Entities;
using ContractService.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContractService.Tests.Services;

public class ContractServiceTests
{
    private readonly Mock<IContractRepository> _repositoryMock;
    private readonly Mock<IProposalStatusChecker> _statusCheckerMock;
    private readonly Mock<ILogger<ContractService.Application.Services.ContractService>> _loggerMock;
    private readonly ContractService.Application.Services.ContractService _service;
    

    public ContractServiceTests()
    {
        _repositoryMock = new Mock<IContractRepository>();
        _statusCheckerMock = new Mock<IProposalStatusChecker>();
        _loggerMock = new Mock<ILogger<ContractService.Application.Services.ContractService>>();
        _service = new ContractService.Application.Services.ContractService(
    _repositoryMock.Object,
    _statusCheckerMock.Object,
    _loggerMock.Object);

    }

    [Fact]
    public void CreateContract_ShouldCreateContractWhenProposalIsApproved()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new CreateContractRequestDTO { ProposalId = proposalId };

        _statusCheckerMock.Setup(x => x.GetStatus(proposalId)).Returns("Approved");
        _repositoryMock.Setup(x => x.GetByProposalId(proposalId)).Returns((Contract)null);

        // Act
        var result = _service.CreateContract(request);

        // Assert
        result.Should().NotBeNull();
        result.ProposalId.Should().Be(proposalId);
        result.ContractDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _repositoryMock.Verify(x => x.Save(It.IsAny<Contract>()), Times.Once);
    }

    [Fact]
    public void CreateContract_ShouldThrowWhenProposalIsNotApproved()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new CreateContractRequestDTO { ProposalId = proposalId };

        _statusCheckerMock.Setup(x => x.GetStatus(proposalId)).Returns("EmAnalise");

        // Act
        Action act = () => _service.CreateContract(request);

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Proposta não está aprovada.");
        _repositoryMock.Verify(x => x.Save(It.IsAny<Contract>()), Times.Never);
    }

    [Fact]
    public void CreateContract_ShouldThrowWhenProposalAlreadyContracted()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new CreateContractRequestDTO { ProposalId = proposalId };

        _statusCheckerMock.Setup(x => x.GetStatus(proposalId)).Returns("Approved");
        _repositoryMock.Setup(x => x.GetByProposalId(proposalId)).Returns(new Contract { ProposalId = proposalId });

        // Act
        Action act = () => _service.CreateContract(request);

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Proposta já contratada.");
        _repositoryMock.Verify(x => x.Save(It.IsAny<Contract>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnContractWhenExists()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var expectedContract = new Contract
        {
            Id = contractId,
            ProposalId = Guid.NewGuid(),
            ContractDate = DateTime.UtcNow
        };
        _repositoryMock.Setup(x => x.GetByIdAsync(contractId)).ReturnsAsync(expectedContract);

        // Act
        var result = await _service.GetByIdAsync(contractId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedContract.Id);
        result.ProposalId.Should().Be(expectedContract.ProposalId);
        result.ContractDate.Should().Be(expectedContract.ContractDate);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullWhenContractDoesNotExist()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByIdAsync(contractId)).ReturnsAsync((Contract)null);

        // Act
        var result = await _service.GetByIdAsync(contractId);

        // Assert
        result.Should().BeNull();
    }
}
