using SyncForge.Api.Application.Interfaces.Services;
using SyncForge.Api.Application.Services;
using SyncForge.Api.Infrastructure;
using SyncForge.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddScoped<IContactService, ContactService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();