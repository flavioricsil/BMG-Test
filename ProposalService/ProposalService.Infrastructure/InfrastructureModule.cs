using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProposalService.Domain.Interfaces;
using ProposalService.Infrastructure.Data;
using ProposalService.Infrastructure.Repositories;

namespace ProposalService.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ProposalDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IProposalRepository, ProposalRepository>();

        return services;
    }
}