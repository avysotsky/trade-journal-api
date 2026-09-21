namespace TradeJournal.Api.Contracts;

public sealed record CreateAccountRequest(string Name, string BaseCurrency);
public sealed record AccountResponse(Guid Id, string Name, string BaseCurrency, DateTimeOffset CreatedAt);
