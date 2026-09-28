using Cribbage.Api.Contracts;
using Cribbage.Api.Data;
using Cribbage.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cribbage.Api.Controllers;

[ApiController, Route("api/games")]
public sealed class GamesController(CribbageDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<GameResponse>>> GetAll([FromQuery] string? player, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 100);
        var query = db.Games.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(player)) query = query.Where(x => EF.Functions.ILike(x.PlayerOne, $"%{player}%") || EF.Functions.ILike(x.PlayerTwo, $"%{player}%"));
        var total = await query.CountAsync(cancellationToken);
        var games = await query.OrderByDescending(x => x.PlayedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return Ok(new PagedResponse<GameResponse>(games.Select(Map).ToArray(), page, pageSize, total));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GameResponse>> Get(Guid id, CancellationToken ct)
    {
        var game = await db.Games.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        return game is null ? NotFound() : Ok(Map(game));
    }

    [HttpPost]
    public async Task<ActionResult<GameResponse>> Create(CreateGameRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerOne) || string.IsNullOrWhiteSpace(request.PlayerTwo)) return BadRequest("Both player names are required.");
        if (request.PlayerOneScore is < 0 or > 121 || request.PlayerTwoScore is < 0 or > 121) return BadRequest("Scores must be between 0 and 121.");
        var game = new Game { PlayerOne = request.PlayerOne.Trim(), PlayerTwo = request.PlayerTwo.Trim(), PlayerOneScore = request.PlayerOneScore, PlayerTwoScore = request.PlayerTwoScore, PlayedAt = request.PlayedAt ?? DateTimeOffset.UtcNow, Notes = request.Notes };
        db.Games.Add(game); await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = game.Id }, Map(game));
    }

    private static GameResponse Map(Game x) => new(x.Id, x.PlayerOne, x.PlayerTwo, x.PlayerOneScore, x.PlayerTwoScore, x.PlayerOneScore == x.PlayerTwoScore ? "Tie" : x.PlayerOneScore > x.PlayerTwoScore ? x.PlayerOne : x.PlayerTwo, x.PlayedAt, x.Notes);
}
