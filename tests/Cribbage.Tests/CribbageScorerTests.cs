using Cribbage.Api.Domain;
using Cribbage.Api.Services;
using Xunit;

namespace Cribbage.Tests;

public sealed class CribbageScorerTests
{
    private readonly CribbageScorer scorer = new();

    [Fact] public void Scores_the_classic_29_hand() => Assert.Equal(29, Score(["5C", "5D", "5H", "JS"], "5S"));
    [Fact] public void Scores_double_run_and_pair() => Assert.Equal(8, Score(["3C", "3D", "4H", "5S"], "9C"));
    [Fact] public void Scores_four_card_flush_outside_crib() => Assert.Equal(4, Score(["2H", "4H", "6H", "8H"], "KC"));
    [Fact] public void Does_not_score_four_card_flush_in_crib() => Assert.Equal(0, Score(["2H", "4H", "6H", "8H"], "KC", true));
    [Fact] public void Rejects_duplicate_cards() => Assert.Throws<ArgumentException>(() => Score(["5C", "5C", "6H", "7S"], "8D"));

    private int Score(string[] hand, string starter, bool crib = false) => scorer.Score(hand.Select(Parse).ToArray(), Parse(starter), crib).Total;
    private static Card Parse(string value) { Assert.True(Card.TryParse(value, out var card)); return card; }
}
