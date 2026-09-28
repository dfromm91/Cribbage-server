namespace Cribbage.Api.Domain;

public sealed class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string PlayerOne { get; set; }
    public required string PlayerTwo { get; set; }
    public int PlayerOneScore { get; set; }
    public int PlayerTwoScore { get; set; }
    public DateTimeOffset PlayedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? Notes { get; set; }
}
