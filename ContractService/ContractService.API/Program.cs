using ContractService.Application.Consumer;
using ContractService.Application.Interfaces;
using ContractService.Application.Mapping;
using ContractService.Application.Validators;
using ContractService.Domain.Interfaces;
using ContractService.Infrastructure.Data;
using ContractService.Infrastructure.HttpClients;
using ContractService.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:8080");

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<ContractDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService.Application.Services.ContractService>();

builder.Services.AddScoped<ProposalApprovedConsumer>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProposalApprovedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["MassTransit:Host"] ?? "rabbitmq";
        var rabbitUser = builder.Configuration["MassTransit:Username"] ?? "guest";
        var rabbitPass = builder.Configuration["MassTransit:Password"] ?? "guest";

        cfg.Host(rabbitHost, "/", h => {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        cfg.ReceiveEndpoint("proposal-approved-queue", e =>
        {
            e.ConfigureConsumer<ProposalApprovedConsumer>(context);
        });
    });
});

var proposalUrl = builder.Configuration["Services:ProposalServiceUrl"];
builder.Services.AddHttpClient<IProposalStatusChecker, ProposalStatusChecker>(client =>
{
    client.BaseAddress = new Uri(proposalUrl);
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateContractRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContractService API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContractDbContext>();

    try
    {
        db.Database.Migrate();
        Console.WriteLine("📦 Migrações aplicadas com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Erro ao aplicar migrações: {ex.Message}");
        Console.WriteLine("➡️ Continuando execução normalmente...");
    }
}

app.Run();
