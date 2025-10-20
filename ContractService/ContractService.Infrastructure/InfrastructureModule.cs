using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ContractService.Domain.Interfaces;
using ContractService.Infrastructure.Data;
using ContractService.Infrastructure.Repositories;

namespace ContractService.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ContractDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IContractRepository, IContractRepository>();

        return services;
    }
}