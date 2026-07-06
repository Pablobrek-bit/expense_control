using System.Text.Json.Serialization;
using ExpenseControl.Application.Interfaces;
using ExpenseControl.Application.Services;
using ExpenseControl.Domain.Interfaces;
using ExpenseControl.Infrastructure.Persistence;
using ExpenseControl.Infrastructure.Persistence.Repositories;
using ExpenseControl.Infrastructure.Web.Middlewares;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


// conexão com o banco de dados postgres
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// repositorios
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// serviços dentro da aplicação
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// controllers e configuração de JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureValidationResponse();

// Tratamento de exceções e ProblemDetails
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

// configuração da parte do cors para poder permitir o uso do backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// essa parte vai aplicar as migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", () => Results.Redirect("/scalar/v1"));
}

app.UseExceptionHandler();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
