using TradeJournal.Api.Models;

namespace TradeJournal.UnitTests;

public sealed class TradeTests
{
    [Fact]
    public void Close_BuyTrade_CalculatesPnlAfterFees()
    {
        var openedAt = DateTimeOffset.Parse("2026-01-01T10:00:00Z");
        var trade = CreateTrade(TradeSide.Buy, openedAt);
        trade.SetOpeningFee(2m);

        trade.Close(110m, 3m, openedAt.AddHours(1));

        Assert.Equal(TradeStatus.Closed, trade.Status);
        Assert.Equal(95m, trade.RealizedPnl);
        Assert.Equal(5m, trade.Fees);
    }

    [Fact]
    public void Close_SellTrade_CalculatesPnlAfterFees()
    {
        var openedAt = DateTimeOffset.Parse("2026-01-01T10:00:00Z");
        var trade = CreateTrade(TradeSide.Sell, openedAt);
        trade.SetOpeningFee(1m);

        trade.Close(90m, 1m, openedAt.AddHours(1));

        Assert.Equal(98m, trade.RealizedPnl);
    }

    [Fact]
    public void Close_AlreadyClosedTrade_Throws()
    {
        var openedAt = DateTimeOffset.Parse("2026-01-01T10:00:00Z");
        var trade = CreateTrade(TradeSide.Buy, openedAt);
        trade.Close(105m, 0m, openedAt.AddMinutes(10));

        Assert.Throws<InvalidOperationException>(() => trade.Close(106m, 0m, openedAt.AddMinutes(20)));
    }

    private static Trade CreateTrade(TradeSide side, DateTimeOffset openedAt) => new()
    {
        AccountId = Guid.NewGuid(),
        Symbol = "BTCUSDT",
        Side = side,
        Quantity = 10m,
        EntryPrice = 100m,
        OpenedAt = openedAt
    };
}
