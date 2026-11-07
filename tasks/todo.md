# Todo: OrderingController → Minimal API

Spec: `SPEC.md` · Plan: `tasks/plan.md` · Endpoint numbers (#) refer to the table in `SPEC.md`.

Build: `~/.dotnet/dotnet build ELIXIR.API/ELIXIR.API.csproj` (0 errors, no new warnings vs. baseline count)
Test:  `~/.dotnet/dotnet test tests/ELIXIR.Tests/ELIXIR.Tests.csproj`

---

## Task 0: ASP.NET Core 7.0 runtime available locally
**Description:** Install the Microsoft.AspNetCore.App 7.0.x runtime into `~/.dotnet` next to SDK 10, so
net7.0 tests run on the same runtime as production.
**Acceptance:**
- [ ] `~/.dotnet/dotnet --list-runtimes` shows `Microsoft.AspNetCore.App 7.0.x` and `Microsoft.NETCore.App 7.0.x`
- [ ] Nothing outside `~/.dotnet` changes
**Verify:** the command above.
**Dependencies:** none (needs approval, see Open Question 1) · **Files:** none in the repo · **Scope:** XS

## Task 1: Test project + factory + smoke test
**Description:** Create the `tests/ELIXIR.Tests` xUnit project (net7.0) with references to `ELIXIR.API` and
packages `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Microsoft.AspNetCore.Mvc.Testing` 7.0.x,
`NSubstitute`. `ElixirApiFactory : WebApplicationFactory<Program>`: environment `Testing`, in-memory config
(dummy `JwtConfig:Key`, dummy connection string), replaces `IUnitOfWork` (whose `.Order` returns a
substitute `IOrdering`) and `IHubContext<OrderHub, IOrderHub>` with substitutes, and creates an `HttpClient`
with the default `Accept: application/json, text/plain, */*`. Also commit `SPEC.md` + `tasks/` on the branch.
**Acceptance:**
- [ ] Smoke test: `GET api/Ordering/GetAllListForScheduleApproval` → 200, and the substitute was called once
- [ ] No SQL Server connection is attempted; no real appsettings/user-secrets are read
**Verify:** test command; build.
**Dependencies:** 0 · **Files:** `tests/ELIXIR.Tests/ELIXIR.Tests.csproj`, `ElixirApiFactory.cs`, `SmokeTests.cs`, `ELIXIR.API.sln` · **Scope:** S

## Task 2: Contract tests, Group A (#1–13)
**Description:** One or more tests per endpoint asserting method + route, status, `Content-Type`, exact raw
body, and the repository calls/`CompleteAsync` count. Cover: J/T/O kinds; `EditOrderQuantity` false → 400 T;
`ValidateNewOrders` success T and 400 with all 7 lists; `AddNewOrder` → empty 200 with no `CompleteAsync`;
`GetAllListOfOrdersPagination` exact `Pagination` + `Access-Control-Expose-Headers`; a null result → 204;
malformed JSON body and empty body on one PUT → capture the current 400 shape.
**Acceptance:**
- [ ] Every endpoint #1–13 has at least one test; all green against the controller
- [ ] Assertions compare exact strings (body, Content-Type, Pagination header), not just status
**Verify:** test command (filter `Group=A`).
**Dependencies:** 1 · **Files:** `tests/ELIXIR.Tests/Ordering/GroupA_OrdersTests.cs`, `TestData.cs` · **Scope:** M

## Task 3: Contract tests, Group B (#14–31)
**Description:** Same approach. Include: #14 query `dateFrom`/`dateTo` + pagination; #15 null → 400 T;
#18/#21/#23 called **without `id`** → 200 and the repository receives 0; #20 echoes the updated `MoveOrder`;
#22 `{ orders, warehouseId }`; #31 echoes the `int[]` with no `CompleteAsync`.
**Acceptance:**
- [ ] Every endpoint #14–31 is covered; the missing-int-param cases are covered; all green against the controller
**Verify:** test command (filter `Group=B`).
**Dependencies:** 1 · **Files:** `tests/ELIXIR.Tests/Ordering/GroupB_MoveOrderTests.cs` · **Scope:** M

## Task 4: Contract tests, Group C (#32–42)
**Description:** Include: the `...Orig` endpoints with `search` absent (falls back to the non-Orig repository
method) and present; #34 with repeated `orderIds` keys and with none → 400 T; #35 empty list falls back to
`...Original`; #41 route `id` + `ReasontDTO` body → 200 T.
**Acceptance:**
- [ ] Every endpoint #32–42 is covered, including both branches of each fallback; all green against the controller
**Verify:** test command (filter `Group=C`).
**Dependencies:** 1 · **Files:** `tests/ELIXIR.Tests/Ordering/GroupC_ApprovalTests.cs` · **Scope:** M

## Task 5: Contract tests, Group D (#43–52)
**Description:** Include: #43 without `status` → false; #45 echoes input with `isActive`/`isTransact` true and
a `preparedDate` set (assert presence and type, not the value); #46/#47 broadcast once via
`Clients.All.SetBeingPrepared` on success, otherwise J message and no broadcast; #51 false → 400 T / true →
empty 200; #52 true → 200 T, false → J.
**Acceptance:**
- [ ] Every endpoint #43–52 is covered, including the SignalR assertions; all green against the controller
**Verify:** test command (filter `Group=D`).
**Dependencies:** 1 · **Files:** `tests/ELIXIR.Tests/Ordering/GroupD_TransactAllocationTests.cs` · **Scope:** M

## Checkpoint 1
- [ ] Full test run green against `OrderingController`; endpoint coverage 52/52; no production files changed
- [ ] Build: no new warnings
- [ ] **Show the user the test list before converting anything**

## Task 6: Helpers + wiring (no endpoints moved)
**Description:** `ELIXIR.API/Http/NewtonsoftResults.cs` with `Json(obj)` (null → 204), `JsonString(msg)`,
`Text(msg)`, `BadRequestText(msg)`, `BadRequestJson(obj)`, all built from MVC's `MvcNewtonsoftJsonOptions`.
`NewtonsoftBody<T>.BindAsync` returns the captured MVC 400 shape for empty/malformed bodies.
`ELIXIR.API/Endpoints/OrderingEndpoints.cs` with an empty `MapOrderingEndpoints` group. In `Startup`:
`AddEndpointsApiExplorer()` and `endpoints.MapOrderingEndpoints()`.
**Acceptance:**
- [ ] Unit tests for each helper (bytes, Content-Type, status, null → 204, bad body → 400 shape)
- [ ] All contract tests still green (nothing has moved yet)
**Verify:** test + build.
**Dependencies:** Checkpoint 1 · **Files:** `Http/NewtonsoftResults.cs`, `Http/NewtonsoftBody.cs`, `Endpoints/OrderingEndpoints.cs`, `Startup.cs`, `tests/.../NewtonsoftResultsTests.cs` · **Scope:** M

## Task 7: Move Group A (#1–13)
**Description:** Map #1–13 in `OrderingEndpoints` and delete those actions from the controller in the same
commit. Handlers keep the original names and the original repository/`CompleteAsync` call order.
Value-type query params get defaults.
**Acceptance:**
- [ ] Group A tests green; groups B–D still green (still served by the controller)
**Verify:** test + build. **Dependencies:** 6 · **Files:** `OrderingEndpoints.cs`, `OrderingController.cs` · **Scope:** S

## Task 8: Move Group B (#14–31)
Same as Task 7 for #14–31. **Acceptance:** all tests green. **Dependencies:** 7 · **Scope:** S

## Task 9: Move Group C (#32–42)
Same as Task 7 for #32–42. The `...Orig` fallbacks call the shared handler. **Acceptance:** all tests green. **Dependencies:** 8 · **Scope:** S

## Task 10: Move Group D (#43–52) + delete the controller
Same as Task 7 for #43–52, with `IHubContext<OrderHub, IOrderHub>` injected into the SetBeingPrepared and
UnsetBeingPrepared handlers. Delete `Controllers/ORDERING_CONTROLLER/OrderingController.cs`.
**Acceptance:**
- [ ] All tests green; the controller file is gone; `grep -r OrderingController ELIXIR.API` finds nothing
**Verify:** test + build. **Dependencies:** 9 · **Scope:** S

## Checkpoint 2
- [ ] All tests green; build has no new warnings; `/moveorder` hub still mapped

## Task 11: Swagger + .http
**Description:** A test fetches `/swagger/v1/swagger.json` and asserts 52 operations under `/api/Ordering/*`
with the right methods, tagged "Ordering". Add `ELIXIR.API/Ordering.http` with one request per endpoint
(port from `launchSettings.json`).
**Acceptance:** [ ] Swagger test green · [ ] `.http` has 52 requests
**Verify:** test + build. **Dependencies:** 10 · **Files:** `tests/.../SwaggerTests.cs`, `ELIXIR.API/Ordering.http` · **Scope:** S

## Task 12: Gap analysis, review, ship report
**Description:** Run `test-gap-analysis` on the diff and close any real gaps. Then run the reviewer round
from CLAUDE.md (`herdr agent prompt reviewer ...` → `tasks/review.md`), fix Critical/Required findings,
re-run all checks (max 2 rounds). Finish with the `/agent-skills:ship` GO/NO-GO report. No push/merge/PR.
**Acceptance:** [ ] review.md has no open Critical/Required findings · [ ] GO/NO-GO report delivered
**Dependencies:** Checkpoint 2, 11 · **Scope:** S
