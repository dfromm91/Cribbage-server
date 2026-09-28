# Cribbage Table

A production-oriented cribbage app with an ASP.NET Core API, Blazor WebAssembly frontend, PostgreSQL, Docker, tests, and a Jenkins-driven Render deployment pipeline.

## Features

- `POST /api/score` scores four-card hands plus a starter, including crib flush rules.
- `GET /api/games`, `GET /api/games/{id}`, and `POST /api/games` query and record PostgreSQL game data.
- Swagger UI at `/swagger` and database-aware health checks at `/health`.
- Responsive Blazor interface for scoring and game history.
- Isolated test and production services/databases in `render.yaml`.

## Run locally

Prerequisites: Docker Desktop with Compose.

```bash
docker compose up --build
```

Open the UI at `http://localhost:3000` or Swagger at `http://localhost:8080/swagger`.

Example scoring request:

```bash
curl -X POST http://localhost:8080/api/score \
  -H 'Content-Type: application/json' \
  -d '{"hand":["5C","5D","5H","JS"],"starter":"5S","isCrib":false}'
```

## CI/CD setup

1. Push this repository to GitHub and create a Render Blueprint from `render.yaml`.
2. In each Render web service, create a deploy hook.
3. In Jenkins, install Pipeline, Docker Pipeline, Credentials Binding, and JUnit plugins.
4. Create a multibranch Pipeline pointed at this repository.
5. Add Secret Text credentials with these exact IDs:
   - `render-test-api-deploy-hook`
   - `render-test-web-deploy-hook`
   - `render-test-url` (the test web root URL)
   - `render-prod-api-deploy-hook`
   - `render-prod-web-deploy-hook`
6. Ensure Jenkins agents can run Docker containers.

Every branch is restored, tested, and built. `main` additionally deploys test, waits for a successful smoke check, requests a manual production approval, then triggers production deploys. Render automatic deploys are disabled so Jenkins is the deployment authority.

## API format

Cards use rank + suit notation. Ranks are `A`, `2`–`10`, `J`, `Q`, `K`; suits are `C`, `D`, `H`, `S`.

```json
{
  "hand": ["3C", "3D", "4H", "5S"],
  "starter": "9C",
  "isCrib": false
}
```

The response contains a total and itemized fifteens, pairs, runs, flush, and nobs score.
