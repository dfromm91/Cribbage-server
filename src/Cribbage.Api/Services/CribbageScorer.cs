using Cribbage.Api.Contracts;
using Cribbage.Api.Domain;

namespace Cribbage.Api.Services;

public interface ICribbageScorer { ScoreHandResponse Score(IReadOnlyList<Card> hand, Card starter, bool isCrib); }

public sealed class CribbageScorer : ICribbageScorer
{
    public ScoreHandResponse Score(IReadOnlyList<Card> hand, Card starter, bool isCrib)
    {
        if (hand.Count != 4) throw new ArgumentException("A cribbage hand must contain exactly four cards.");
        var cards = hand.Append(starter).ToArray();
        if (cards.Distinct().Count() != 5) throw new ArgumentException("Cards must be unique.");

        var fifteens = 0;
        for (var mask = 1; mask < 1 << cards.Length; mask++)
            if (Enumerable.Range(0, cards.Length).Where(i => (mask & (1 << i)) != 0).Sum(i => cards[i].PipValue) == 15)
                fifteens += 2;

        var pairs = cards.GroupBy(c => c.Rank).Sum(g => g.Count() * (g.Count() - 1));
        var runs = ScoreRuns(cards);
        var sameSuit = hand.All(c => c.Suit == hand[0].Suit);
        var flush = sameSuit ? (starter.Suit == hand[0].Suit ? 5 : isCrib ? 0 : 4) : 0;
        var nobs = hand.Any(c => c.Rank == 11 && c.Suit == starter.Suit) ? 1 : 0;
        var breakdown = new ScoreBreakdown(fifteens, pairs, runs, flush, nobs);
        return new ScoreHandResponse(fifteens + pairs + runs + flush + nobs, breakdown, cards.Select(c => c.ToString()).ToArray(), isCrib);
    }

    private static int ScoreRuns(IReadOnlyList<Card> cards)
    {
        for (var length = 5; length >= 3; length--)
        {
            var count = 0;
            foreach (var combo in Combinations(cards, length))
                if (combo.Select(c => c.Rank).Distinct().Count() == length && combo.Max(c => c.Rank) - combo.Min(c => c.Rank) == length - 1)
                    count++;
            if (count > 0) return count * length;
        }
        return 0;
    }

    private static IEnumerable<IReadOnlyList<Card>> Combinations(IReadOnlyList<Card> cards, int size)
    {
        for (var mask = 0; mask < 1 << cards.Count; mask++)
            if (Enumerable.Range(0, cards.Count).Count(i => (mask & (1 << i)) != 0) == size)
                yield return Enumerable.Range(0, cards.Count).Where(i => (mask & (1 << i)) != 0).Select(i => cards[i]).ToArray();
    }
}
