namespace Cribbage.Api.Contracts;

public sealed record ScoreHandRequest(IReadOnlyList<string> Hand, string Starter, bool IsCrib = false);
public sealed record ScoreBreakdown(int Fifteens, int Pairs, int Runs, int Flush, int Nobs);
public sealed record ScoreHandResponse(int Total, ScoreBreakdown Breakdown, IReadOnlyList<string> Cards, bool IsCrib);
