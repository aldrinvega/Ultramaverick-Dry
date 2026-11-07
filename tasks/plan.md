# Implementation Plan: OrderingController → Minimal API

Spec: `SPEC.md` (approved 2026-11-07). Status: **DRAFT, waiting for "approve plan"**

## Overview

Two steps, in this order:
1. Write contract tests that pin down every one of the 52 Ordering endpoints, and run them **against the current controller** until they pass.
2. Move the endpoints into Minimal APIs one group at a time. After each move, the same tests must still pass.

The tests make HTTP calls through `WebApplicationFactory`, with `IUnitOfWork`/`IOrdering` and the
SignalR hub context replaced by fakes. They don't care how an endpoint is implemented, so the same tests
check the controller and the Minimal API version.

Branch `feature/ordering-minimal-api`, worktree `.worktrees/ordering-minimal-api`. One commit per task.

## Architecture Decisions

- **Contract tests come first.** These tests are the definition of "unchanged". They're written and passing
  before any production file is touched.
- **Endpoints move one group at a time.** Each group's actions are removed from `OrderingController` in
  the same commit that maps them in `OrderingEndpoints`. A route never exists twice, and the app works
  after every commit. The last group deletes the controller file.
- **MVC reuse inside Minimal API handlers** (approved 2026-11-07 at Checkpoint 1, replacing hand-written
  Newtonsoft helpers). Routes are plain `MapGet/MapPut/MapPost` in `OrderingEndpoints`. Handlers:
  - bind query/route/body values with MVC's `ParameterBinder` + value providers + input formatters
    (`ELIXIR.API/Http/MvcRequestBinder.cs`), sharing one `ActionContext`/`ModelStateDictionary` per request.
    Bad input yields the same responses as the `[ApiController]` filters: an `UnsupportedContentTypeException`
    becomes 415 via `IClientErrorFactory`, and invalid model state goes through `ApiBehaviorOptions.InvalidModelStateResponseFactory`.
  - return the same MVC `IActionResult`s as today (`OkObjectResult`, `JsonResult`, `OkResult`, `BadRequestObjectResult`),
    executed through an `IResult` adapter (`ELIXIR.API/Http/MvcResult.cs`). This gives identical formatters,
    content negotiation and the null → 204 behavior.
  - Swagger: parameters are declared as endpoint metadata (`WithMvcParameters`) and rendered by a Swashbuckle
    operation filter. Bodies use `.Accepts<T>()`. A Swagger test pins the parameters/bodies captured from the controller.
  Probing showed why: MVC's error responses (multiple Newtonsoft messages, 415 ProblemDetails, "is not valid"
  messages for query values, combined route+body errors) are too varied to copy reliably by hand.
- **Tests run on the .NET 7 runtime**, the same as production (`mcr.microsoft.com/dotnet/aspnet:7.0`). This
  Mac only has .NET 10. Rolling forward to 10 would test Minimal API binding on a different framework
  version than the one that ships. Plan: install the ASP.NET Core 7.0 runtime into `~/.dotnet`
  (Task 0, `setup-local-sdk` skill). Fallback if you say no: `DOTNET_ROLL_FORWARD=Major`, with the gap
  stated in the report.
- **Which `dotnet-webapi` skill guidance applies.** Used: static `Map<Resource>` class + `MapGroup`, Swagger
  metadata (`WithTags`, `WithName`), a `.http` file, keeping Swashbuckle (net7). **Not used, on purpose**,
  because each one would change the contract the spec freezes: `TypedResults`/201 Created, ProblemDetails,
  record DTOs instead of entities, `DateTimeOffset`, string enums, `CancellationToken` (the repositories
  don't accept tokens), a service layer.

## Task List (details in `tasks/todo.md`)

### Phase 0: Environment
- [x] Task 0: ASP.NET Core 7.0 runtime available locally

### Phase 1: Test harness
- [x] Task 1: Scaffold `tests/ELIXIR.Tests` + `ElixirApiFactory` + one smoke test

### Phase 2: Contract tests against the existing controller (no production changes)
- [x] Task 2: Group A — orders & scheduling (#1–13)
- [x] Task 3: Group B — move-order preparation & approval (#14–31)
- [x] Task 4: Group C — move-order pagination, print, view (#32–42)
- [x] Task 5: Group D — transact, SignalR, allocation (#43–52)

### Checkpoint 1
- [x] All contract tests are green against `OrderingController` (100 tests; Group E added for binding errors); build has no new warnings; **stop and show you the test list**

### Phase 3: Conversion
- [x] Task 6: Swagger contract test (captured from controller) + MVC reuse helpers + Swagger filter + `AddEndpointsApiExplorer` + empty `MapOrderingEndpoints`
- [x] Task 7: Move Group A
- [x] Task 8: Move Group B
- [x] Task 9: Move Group C
- [x] Task 10: Move Group D + delete `OrderingController.cs`

### Checkpoint 2
- [x] All contract tests still green; controller gone; no new warnings (157 tests; 142 warnings)

### Phase 4: Finish
- [x] Task 11: Swagger test (52 operations under `/api/Ordering`) + `ELIXIR.API/Ordering.http`
- [ ] Task 12: `test-gap-analysis`, then a reviewer round (CLAUDE.md workflow, max 2), then the `/agent-skills:ship` GO/NO-GO report

## Skills to load (CLAUDE.md table)

| Task | Skills |
|---|---|
| 0 | `dotnet:setup-local-sdk` |
| 1 | `dotnet-test:scaffold-dotnet-test-project`, `dotnet-test:run-tests` |
| 2–5 | `dotnet-test:code-testing`, `dotnet-test:run-tests` |
| 6–10 | `dotnet-aspnetcore:dotnet-webapi`, `dotnet:csharp-refactoring` (moving behavior unchanged) |
| 11 | `dotnet-aspnetcore:dotnet-webapi` |
| 12 | `dotnet-test:test-gap-analysis` |

`optimizing-ef-core-queries` is not needed, because no queries or repositories change.

## Risks and Mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| Minimal APIs serialize with System.Text.Json | High | Newtonsoft helpers built from MVC's own options; contract tests compare exact bodies including dates |
| `Ok(string)` text/plain vs `JsonResult` JSON mixed up | High | Every test sends `Accept: application/json, text/plain, */*` and asserts `Content-Type` + raw body |
| Missing `int`/`bool` query params now give 400 | High | A test per value-type param calls without it and expects 200 with 0/false passed to the repository |
| `null` repository result → 200 instead of 204 | Med | Helper maps null → 204; tests for each `O` endpoint with a null result |
| Malformed/empty body response changes | Med | Captured from MVC in Phase 2, then reproduced by `NewtonsoftBody<T>` |
| `Pagination` header changes (including the swapped next/prev) | Med | Tests assert the exact header string |
| Startup needs `JwtConfig:Key` + connection string | Low | Factory runs in environment `Testing` with in-memory dummy values. No real `appsettings`/user-secrets are read, and SQL Server is never contacted |
| Tests run on a different runtime than production | Med | Task 0 (.NET 7 runtime) |

## Open Questions

1. **Task 0:** OK to install the ASP.NET Core **7.0** runtime (out of support since May 2024, but it's what
   production runs) into `~/.dotnet` with Microsoft's `dotnet-install.sh`? It goes next to .NET 10 and
   doesn't change the system install. If not, I'll use roll-forward to 10 and note the gap.
2. Add `tests/ELIXIR.Tests` to `ELIXIR.API.sln` too? (Recommended: yes. We still build the `.csproj` files,
   not the solution.)
