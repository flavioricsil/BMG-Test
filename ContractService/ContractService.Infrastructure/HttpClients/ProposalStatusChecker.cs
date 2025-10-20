using System.Net.Http;
using System.Net.Http.Json;
using ContractService.Application.Interfaces;

namespace ContractService.Infrastructure.HttpClients;

public class ProposalStatusChecker : IProposalStatusChecker
{
    private readonly HttpClient _httpClient;

    public ProposalStatusChecker(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string GetStatus(Guid proposalId)
    {
        var response = _httpClient.GetAsync($"/api/Proposal/{proposalId}").Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao consultar proposta {proposalId}: {response.StatusCode} - {response.ReasonPhrase}");
        }

        try
        {
            var proposta = response.Content.ReadFromJsonAsync<ProposalResponse>().Result;

            if (proposta == null || string.IsNullOrWhiteSpace(proposta.Status))
            {
                throw new Exception("Resposta da proposta está vazia ou inválida.");
            }

            return proposta.Status;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao desserializar resposta da proposta {proposalId}: {ex.Message}", ex);
        }
    }

    private class ProposalResponse
    {
        public string Status { get; set; }
    }
}