using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProposalService.Application.Interfaces;
using ProposalService.Application.Mapping;
using ProposalService.Application.Validators;
using ProposalService.Domain.Interfaces;
using ProposalService.Infrastructure.Data;
using ProposalService.Infrastructure.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Força escutar na porta 80 (compatível com Docker mapeando "500x:80")
builder.WebHost.UseUrls("http://*:8080");

// Configuração
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Banco de Dados PostgreSQL
builder.Services.AddDbContext<ProposalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositórios e serviços
builder.Services.AddScoped<IProposalRepository, ProposalRepository>();
builder.Services.AddScoped<IProposalService, ProposalService.Application.Services.ProposalService>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProposalRequestDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddMassTransitHostedService();

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProposalService API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Migrações automáticas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProposalDbContext>();

    try
    {
        db.Database.Migrate();
        Console.WriteLine("Migrações aplicadas com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao aplicar migrações: {ex.Message}");
        Console.WriteLine("Continuando execução normalmente...");
    }
}

app.Run();