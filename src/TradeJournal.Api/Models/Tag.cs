namespace TradeJournal.Api.Models;

public sealed class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public List<Trade> Trades { get; set; } = [];
}
