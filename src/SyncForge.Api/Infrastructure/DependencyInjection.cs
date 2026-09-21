using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SyncForge.Api.Application.Interfaces.Persistence;
using SyncForge.Api.Infrastructure.Data.MongoDB;
using SyncForge.Api.Infrastructure.Data.MongoDB.Documents;
using SyncForge.Api.Infrastructure.Repositories;

namespace SyncForge.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<MongoDbOptions>()
            .Bind(configuration.GetSection(MongoDbOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                "MongoDb:ConnectionString deve ser configurada.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.DatabaseName),
                "MongoDb:DatabaseName deve ser configurado.")
            .ValidateOnStart();

        services.AddSingleton<IMongoClient>(provider =>
        {
            var options = provider
                .GetRequiredService<IOptions<MongoDbOptions>>()
                .Value;

            return new MongoClient(options.ConnectionString);
        });

        services.AddSingleton<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();

            var options = provider
                .GetRequiredService<IOptions<MongoDbOptions>>()
                .Value;

            return client.GetDatabase(options.DatabaseName);
        });

        services.AddSingleton<IMongoCollection<ContactDocument>>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();

            return database.GetCollection<ContactDocument>(
                ContactDocument.CollectionName);
        });

        services.AddScoped<IContactRepository, MongoContactRepository>();

        services.AddHostedService<MongoDbInitializer>();

        return services;
    }
}