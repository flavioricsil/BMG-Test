using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;

namespace ProposalService.Infrastructure.Data;

public class ProposalDbContext : DbContext
{
    public ProposalDbContext(DbContextOptions<ProposalDbContext> options) : base(options) { }

    public DbSet<Proposal> Proposals => Set<Proposal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.CustomerName).IsRequired();
            entity.Property(p => p.InsuranceType).IsRequired();
            entity.Property(p => p.Status).HasConversion<string>();
            entity.Property(p => p.CreatedAt);
        });
    }
}