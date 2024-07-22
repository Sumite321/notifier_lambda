using AutoFixture;
using FluentAssertions;
using Moq;
using Notifier.Lambda.Infrastructure.Redis;
using StackExchange.Redis;
using Xunit;

namespace ClassLibrary1;

public class RedisServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;
    private readonly RedisService _redisService;

    public RedisServiceTests()
    {
        _fixture = new Fixture();
        _mockDatabase = new Mock<IDatabase>();
        _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();

        _mockConnectionMultiplexer.Setup(conn => conn.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _redisService = new RedisService(_mockConnectionMultiplexer.Object);
    }

    [Fact]
    public async Task HasBeenProcessedAsync_ShouldReturnTrueIfKeyExists()
    {
        var messageId = _fixture.Create<string>();

        _mockDatabase.Setup(db => db.KeyExistsAsync(messageId, It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var result = await _redisService.HasBeenProcessedAsync(messageId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasBeenProcessedAsync_ShouldReturnFalseIfKeyDoesNotExist()
    {
        var messageId = _fixture.Create<string>();

        _mockDatabase.Setup(db => db.KeyExistsAsync(messageId, It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        var result = await _redisService.HasBeenProcessedAsync(messageId);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task MarkAsProcessedAsync_ShouldSetKey()
    {
        var messageId = _fixture.Create<string>();

        await _redisService.MarkAsProcessedAsync(messageId);

        _mockDatabase.Verify(db => db.StringSetAsync(messageId, "processed", null, false, When.NotExists, CommandFlags.None));
    }
}