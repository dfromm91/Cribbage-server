using Cribbage.Api.Contracts;
using Cribbage.Api.Domain;
using Cribbage.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cribbage.Api.Controllers;

[ApiController, Route("api/score")]
public sealed class ScoreController(ICribbageScorer scorer) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ScoreHandResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ScoreHandResponse> Score(ScoreHandRequest request)
    {
        if (request.Hand is null || request.Hand.Count != 4)
            ModelState.AddModelError(nameof(request.Hand), "Provide exactly four cards.");
        var values = (request.Hand ?? []).Append(request.Starter).ToArray();
        var cards = new List<Card>();
        foreach (var value in values)
            if (Card.TryParse(value, out var card)) cards.Add(card);
            else ModelState.AddModelError("cards", $"'{value}' is invalid. Use ranks A, 2-10, J, Q, K and suits C, D, H, S (for example 5H).");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try { return Ok(scorer.Score(cards[..4], cards[4], request.IsCrib)); }
        catch (ArgumentException ex) { ModelState.AddModelError("cards", ex.Message); return ValidationProblem(ModelState); }
    }
}
