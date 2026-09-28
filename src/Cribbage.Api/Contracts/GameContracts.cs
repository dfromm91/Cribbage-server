namespace Cribbage.Api.Contracts;

public sealed record CreateGameRequest(string PlayerOne, string PlayerTwo, int PlayerOneScore, int PlayerTwoScore, DateTimeOffset? PlayedAt, string? Notes);
public sealed record GameResponse(Guid Id, string PlayerOne, string PlayerTwo, int PlayerOneScore, int PlayerTwoScore, string Winner, DateTimeOffset PlayedAt, string? Notes);
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);
