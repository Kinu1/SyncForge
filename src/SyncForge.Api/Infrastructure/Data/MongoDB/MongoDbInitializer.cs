using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SyncForge.Api.Infrastructure.Data.MongoDB.Documents;

namespace SyncForge.Api.Infrastructure.Data.MongoDB;

public sealed class MongoDbInitializer : IHostedService
{
    private readonly IMongoCollection<ContactDocument> _contacts;
    private readonly ILogger<MongoDbInitializer> _logger;

    public MongoDbInitializer(
        IMongoCollection<ContactDocument> contacts,
        ILogger<MongoDbInitializer> logger)
    {
        _contacts = contacts;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var indexKeys = Builders<ContactDocument>
            .IndexKeys
            .Ascending(contact => contact.Email);

        var indexOptions = new CreateIndexOptions
        {
            Name = "ux_contacts_email",
            Unique = true
        };

        var index = new CreateIndexModel<ContactDocument>(
            indexKeys,
            indexOptions);

        await _contacts.Indexes.CreateOneAsync(
            index,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "MongoDB pronto: índice único de e-mail disponível.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}