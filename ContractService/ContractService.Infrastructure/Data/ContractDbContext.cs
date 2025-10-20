using ContractService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Infrastructure.Data;

public class ContractDbContext : DbContext
{
    public ContractDbContext(DbContextOptions<ContractDbContext> options) : base(options) { }

    public DbSet<Contract> Contracts { get; set; }
}