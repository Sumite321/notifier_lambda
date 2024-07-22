using StackExchange.Redis;

namespace Notifier.Lambda.Infrastructure.Redis;

public class RedisService(IConnectionMultiplexer connectionMultiplexer) : IRedisService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public async Task<bool> HasBeenProcessedAsync(string messageId)
    {
        return await _database.KeyExistsAsync(messageId);
    }

    public async Task MarkAsProcessedAsync(string messageId)
    {
        await _database.StringSetAsync(messageId, "processed", when: When.NotExists);
    }
}