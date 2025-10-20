using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ContractService.Infrastructure.HttpClients;
using Moq;
using Moq.Protected;
using Xunit;

namespace ContractService.Tests.Infrastructure.HttpClients;

public class ProposalStatusCheckerTests
{
    [Fact]
    public void Should_Return_Status_When_Proposal_Exists()
    {
        // Arrange
        var expectedStatus = "Approved";
        var proposalId = Guid.NewGuid();

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().EndsWith($"/api/Proposal/{proposalId}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new ProposalResponse
                {
                    Id = proposalId,
                    Status = expectedStatus
                })
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5001")
        };

        var checker = new ProposalStatusChecker(httpClient);

        // Act
        var status = checker.GetStatus(proposalId);

        // Assert
        Assert.Equal(expectedStatus, status);
    }

    private class ProposalResponse
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }
}