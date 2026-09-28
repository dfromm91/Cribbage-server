namespace Cribbage.Api.Domain;

public enum Suit { Clubs, Diamonds, Hearts, Spades }

public readonly record struct Card(int Rank, Suit Suit)
{
    public int PipValue => Math.Min(Rank, 10);

    public static bool TryParse(string value, out Card card)
    {
        card = default;
        if (string.IsNullOrWhiteSpace(value) || value.Length is < 2 or > 3) return false;
        var rankText = value[..^1].ToUpperInvariant();
        var suitText = char.ToUpperInvariant(value[^1]);
        var rank = rankText switch { "A" => 1, "J" => 11, "Q" => 12, "K" => 13, _ => int.TryParse(rankText, out var n) ? n : 0 };
        var suit = suitText switch { 'C' => Suit.Clubs, 'D' => Suit.Diamonds, 'H' => Suit.Hearts, 'S' => Suit.Spades, _ => (Suit)(-1) };
        if (rank is < 1 or > 13 || !Enum.IsDefined(suit)) return false;
        card = new Card(rank, suit);
        return true;
    }

    public override string ToString() => $"{Rank switch { 1 => "A", 11 => "J", 12 => "Q", 13 => "K", _ => Rank.ToString() }}{Suit.ToString()[0]}";
}
