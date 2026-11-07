# Spec: Convert `OrderingController` to Minimal API

Status: **APPROVED 2026-11-07**

## Objective

Replace the MVC `OrderingController` (`ELIXIR.API/Controllers/ORDERING_CONTROLLER/OrderingController.cs`,
52 actions) with Minimal API endpoints. **The frontend must not notice the change.** For every endpoint,
these stay exactly the same: HTTP method, URL, query/route/body binding, status code, `Content-Type`,
response body, and the `Pagination` header.

Who uses it: the existing ELIXIR frontend (ordering, move-order, transact and allocation screens) and the
SignalR clients listening on `/moveorder`.

Out of scope: `CancelledOrdersController`, the MediatR ordering slices in `ELIXIR.DATA`, repository logic,
auth/CORS, and changing any other controller. Known bugs stay as they are (see "Preserve as-is").

### Assumptions (correct me now or I'll proceed with these)

1. Only `OrderingController` is converted. The other controllers stay MVC.
2. The controller file is **deleted**. If both existed, the same routes would be registered twice and every
   request would fail with `AmbiguousMatchException`.
3. Responses keep **Newtonsoft.Json** with the current MVC settings (camelCase, `ReferenceLoopHandling.Ignore`).
   By default Minimal APIs use System.Text.Json and ignore `AddNewtonsoftJson`. That would change date
   formats and how loops in entity graphs are handled, and stricter date parsing would reject some request
   bodies. Request bodies are also read with Newtonsoft.
4. No repository, DTO, entity or `IUnitOfWork` changes. Handlers call the same repository methods in the
   same order, and call `CompleteAsync()` in the same places (including the double `CompleteAsync` in
   `ValidateNewOrders` and the missing save in `AddNewOrder` / `UpdatePrintStatus`).
5. Endpoints stay anonymous (no `RequireAuthorization`), like the controller today.

## Tech Stack

- ASP.NET Core **net7.0** (`MapGroup`, `TypedResults`, `[AsParameters]` are available), Startup-style hosting.
- Newtonsoft.Json via `Microsoft.AspNetCore.Mvc.NewtonsoftJson` 7.0.16 (already referenced by `ELIXIR.DATA`).
- Swashbuckle 6.3.1 (needs `services.AddEndpointsApiExplorer()` to show Minimal API endpoints).
- No new NuGet packages for the production code.

## Endpoint contract (must not change)

Base path: `api/Ordering` (route matching is case-insensitive in both MVC and Minimal APIs. Keep the
original spelling anyway, e.g. `GetAllListofOrders`).

How each response looks today:
- **J**: `new JsonResult("msg")` → 200, `application/json`, body is a quoted JSON string.
- **T**: `Ok("msg")` / `BadRequest("msg")` → MVC's `StringOutputFormatter` → `text/plain; charset=utf-8`,
  body is the raw text with no quotes. This applies when the request's `Accept` includes `*/*` or `text/plain`,
  which covers the browser/axios default.
- **O**: `Ok(obj)` → 200, `application/json` (Newtonsoft). **If `obj` is `null`, MVC returns 204 with an empty body.**
- **E**: `Ok()` → 200, empty body.
- **P**: paginated. `Pagination` header + `Access-Control-Expose-Headers` set by `Response.AddPaginationHeader(...)`
  (called with exactly the same arguments as today), plus an `O` body
  `{ <listName>, currentPage, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage }`.
  `<listName>` is `orders` or `moveorder`, as today.

| # | Method | Route | Inputs | Responses |
|---|---|---|---|---|
| 1 | GET | GetAllListofOrders | q `farms` | O |
| 2 | PUT | EditOrderQuantity | body `Ordering` | 400 T / J |
| 3 | PUT | SchedulePreparedOrderedDate | body `Ordering[]` | J |
| 4 | PUT | ApprovePreparedDate | body `List<Ordering>` | J |
| 5 | PUT | RejectPreparedDate | body `List<Ordering>` | J |
| 6 | PUT | GetAllListofPreparedDate | — | J |
| 7 | GET | OrderSummary | q `dateFrom`, `dateTo` | O |
| 8 | POST | ValidateNewOrders | body `Ordering[]` | 200 T / 400 O (7 lists) |
| 9 | POST | AddNewOrder | body `Ordering[]` | E |
| 10 | GET | GetAllListOfOrdersPagination | q `UserParams` | P (`orders`) |
| 11 | PUT | CancelOrders | body `Ordering[]` | 400 T / 200 T |
| 12 | GET | GetAllListOfCancelledOrders | — | O |
| 13 | PUT | ReturnCancelledOrders | body `Ordering` | 400 T / 200 T |
| 14 | GET | GetAllListForMoveOrderPagination | q `UserParams`, `dateTo`, `dateFrom` | P (`orders`) |
| 15 | GET | GetAllListOfApprovedPreparedForMoveOrder | q `farm` | null → 400 T / O |
| 16 | GET | DetailedListOfOrders | q `farm` | O |
| 17 | GET | GetAllListForScheduleApproval | — | O |
| 18 | GET | GetAllOrdersForScheduleApproval | q `id` int | O |
| 19 | GET | GetAllOutOfStockByItemCodeAndOrderDate | q `itemcode`, `orderdate` | O |
| 20 | POST | PrepareItemsForMoveOrder | body `MoveOrder` | O (the updated order) |
| 21 | GET | GetAllListOfOrdersForMoveOrder | q `id` int | O |
| 22 | GET | GetAvailableStockFromWarehouse | q `id` int, `itemcode` | O `{ orders, warehouseId }` |
| 23 | GET | ListOfPreparedItemsForMoveOrder | q `id` int | O |
| 24 | PUT | CancelPreparedItems | body `MoveOrder` | 200 T |
| 25 | PUT | AddPlateNumberInMoveOrder | body `Ordering[]` | J |
| 26 | PUT | AddDeliveryStatus | body `OrderDto[]` | J |
| 27 | PUT | ApproveListOfMoveOrder | body `MoveOrder[]` | J |
| 28 | PUT | RejectListOfMoveOrder | body `MoveOrder` | J |
| 29 | PUT | RejectApproveListOfMoveOrder | body `MoveOrder` | J |
| 30 | PUT | ReturnMoveOrderForApproval | body `MoveOrder` | J |
| 31 | PUT | UpdatePrintStatus | body `int[]` | O (echoes input) |
| 32 | GET | GetAllForApprovalMoveOrderPagination | q `UserParams` | P (`moveorder`) |
| 33 | GET | GetAllForApprovalMoveOrderPaginationOrig | q `UserParams`, `search` | `search` null → same as #32, else P |
| 34 | GET | PrintMultipleMoveOrderSlip | q `orderIds` (repeated) | empty → 400 T / O |
| 35 | GET | ViewMoveOrderForApproval | q `id` int | O (falls back to `...Original` when the list is empty) |
| 36 | GET | ApprovedMoveOrderPagination | q `UserParams`, `dateFrom`, `dateTo` | P (`moveorder`) |
| 37 | GET | ApprovedMoveOrderPaginationOrig | q `UserParams`, `search`, `dateFrom`, `dateTo` | `search` null → same as #36, else P |
| 38 | GET | RejectedMoveOrderPagination | q `UserParams` | P (`moveorder`) |
| 39 | GET | RejectedMoveOrderPaginationOrig | q `UserParams`, `search` | `search` null → same as #38, else P |
| 40 | GET | GetAllApprovedMoveOrder | q `id` int | O |
| 41 | PUT | CancelOrdersInMoveOrder/{id} | route `id` int, body `ReasontDTO` | 200 T |
| 42 | GET | GetAllApprovedOrderCalendar | — | O |
| 43 | GET | GetTotalListForMoveOrder | q `status` bool | O |
| 44 | GET | ListOfMoveOrdersForTransact | q `orderid` int | O |
| 45 | POST | TransactListOfMoveOrders | body `TransactMoveOrder[]` | O (echoes input after setting flags/date) |
| 46 | POST | SetBeingPrepared | body `Ordering[]` | E + SignalR broadcast / J "Someone is already preparing!" |
| 47 | POST | UnsetBeingPrepared | body `Ordering[]` | E + SignalR broadcast / J "Something Wrong" |
| 48 | PUT | Allocate | body `AllocationFinalResult` | O |
| 49 | GET | GetAllListOfOrdersForAllocationPagination | q `UserParams` | P (`orders`) |
| 50 | GET | GetAllListofOrdersForAllocation | q `itemCode` | O |
| 51 | POST | CancelForPendingAllocation | q `customer` | 400 T / E |
| 52 | PUT | ManualAllocation | body `List<ManualAllocation>` | 200 T / J |

`UserParams` = query `PageNumber` (default 1), `PageSize` (default 10, max 2000).

### Binding traps (Minimal APIs behave differently from MVC here)

- **Missing value-type query params** (`int id`, `bool status`, `int orderid`): MVC binds them to `0`/`false`;
  Minimal APIs return 400. Declare them with a default (`int id = 0`) or as `int?` and use `?? 0`.
- **Reference-type query params**: nullable context is off in both projects, so a missing value is `null`, same as today.
- **`UserParams`**: bind with `[AsParameters]` and keep the `PageSize` clamp.
- **`List<int> orderIds`**: repeated query keys (`?orderIds=1&orderIds=2`) must still bind.
- **`null` results**: `Ok(null)` → 204 has to be reproduced (MVC's `HttpNoContentOutputFormatter`), not turned into a 200 with `null`.
- **Malformed or empty JSON body**: today's status and body for this case get captured in the baseline and must be kept.
- **Mixed result types**: endpoints that can return either T or J must keep returning each one in the same case as today.

### Preserve as-is (known quirks; don't fix in this change)

- `AddPaginationHeader` is called with `HasNextPage, HasPreviousPage`, but its parameters are declared
  `(hasPreviousPage, hasNextPage)`, so the header has those two values swapped. Keep the call exactly as it is.
- `GetAllListofPreparedDate` is a PUT that returns a fixed message.
- `TransactListOfMoveOrders` uses `DateTime.Now`.
- Only `item` is broadcast, and only the first successful one in `SetBeingPrepared` / `UnsetBeingPrepared`.

## Commands

```bash
~/.dotnet/dotnet restore ELIXIR.API/ELIXIR.API.csproj
~/.dotnet/dotnet build   ELIXIR.API/ELIXIR.API.csproj   # 0 errors, no new warnings (baseline ~140)
~/.dotnet/dotnet run --project ELIXIR.API               # /swagger → check the "Ordering" group
~/.dotnet/dotnet test    tests/ELIXIR.Tests             # only if Open Question 1 = B
```

## Project Structure

```
ELIXIR.API/
  Endpoints/
    OrderingEndpoints.cs        → static MapOrderingEndpoints(this IEndpointRouteBuilder) — MapGroup("api/Ordering"), 52 Map* calls, private static handlers
  Http/
    NewtonsoftResults.cs        → IResult helpers: Json(obj) (null → 204), JsonString(msg), Text(msg)/BadRequestText(msg), BadRequestJson(obj)
                                   + Newtonsoft body reader/binder, using the same SerializerSettings MVC uses
  Startup.cs                    → AddEndpointsApiExplorer(); endpoints.MapOrderingEndpoints() inside UseEndpoints
  Controllers/ORDERING_CONTROLLER/OrderingController.cs → DELETED
tests/ELIXIR.Tests/             → only if approved (Open Question 1)
```

## Code Style

Match the repo: same namespaces style, action-named routes, `IUnitOfWork` injected into each handler,
the handler calls `CompleteAsync()`. Handlers are named after the old actions so diffs are easy to compare.

```csharp
public static class OrderingEndpoints
{
    public static IEndpointRouteBuilder MapOrderingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/Ordering").WithTags("Ordering");

        group.MapPut("EditOrderQuantity", EditOrderQuantity);
        group.MapGet("GetAllOrdersForScheduleApproval", GetAllOrdersForScheduleApproval);
        group.MapGet("GetAllListOfOrdersPagination", GetAllListOfOrdersPagination);
        // ...
        return app;
    }

    private static async Task<IResult> EditOrderQuantity(NewtonsoftBody<Ordering> order, IUnitOfWork unitOfWork)
    {
        var result = await unitOfWork.Order.EditQuantityOrder(order.Value);
        if (result == false)
            return NewtonsoftResults.BadRequestText("Quantity must not transcend the value of allocated quantity!");
        await unitOfWork.CompleteAsync();
        return NewtonsoftResults.JsonString("Successfully edit order quantity!");
    }

    private static async Task<IResult> GetAllOrdersForScheduleApproval(IUnitOfWork unitOfWork, int id = 0)
        => NewtonsoftResults.Json(await unitOfWork.Order.GetAllOrdersForScheduleApproval(id));

    private static async Task<IResult> GetAllListOfOrdersPagination(
        [AsParameters] UserParams userParams, HttpResponse response, IUnitOfWork unitOfWork)
    {
        var orders = await unitOfWork.Order.GetAllListofOrdersPagination(userParams);
        response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages,
            orders.HasNextPage, orders.HasPreviousPage);   // argument order kept on purpose
        return NewtonsoftResults.Json(new { orders, orders.CurrentPage, orders.PageSize, orders.TotalCount,
            orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage });
    }
}
```

(`NewtonsoftBody<T>` is a type with a `BindAsync` method that reads the body with Newtonsoft. The exact
shape gets decided in the plan.)

## Testing Strategy

The test is a **before/after contract comparison**. For every endpoint, record what the current
controller returns (status, `Content-Type`, `Pagination` header, body), then check that the Minimal API
returns exactly the same thing.

- **Option A (no new project):** a script in `tasks/contract/` calls each endpoint on a locally running API
  (needs `appsettings.json` with a dev DB), once on `main` and once on the branch, and diffs the output.
  Only GETs run against a real DB. PUT/POST are checked by hand in Swagger. Weaker, and depends on the data.
- **Option B (recommended):** add `tests/ELIXIR.Tests` (xUnit + `Microsoft.AspNetCore.Mvc.Testing` + a
  mocking library). It boots the real `Startup` through `WebApplicationFactory` with a fake `IUnitOfWork` /
  `IOrdering...` repo and a fake `IHubContext`, and has one test per row of the table, including the null →
  204, T vs J, missing-`id`, pagination-header and SignalR-broadcast cases. **Write these tests first,
  against the controller (green), then convert, and they must still pass.** No SQL Server. Needs approval
  for the new project and its packages (CLAUDE.md requires asking first).

In both cases, also: the build has no new warnings, and Swagger lists all 52 Ordering operations.

## Boundaries

- **Always:** keep every route, method, binding, status, content type, body and header byte-for-byte; call
  repositories and `CompleteAsync()` exactly like the controller does; build the `.csproj`; one small commit
  per task on a feature branch in `.worktrees/<branch>`.
- **Ask first:** adding the test project or any NuGet package; moving the app's JSON to System.Text.Json;
  fixing any quirk listed under "Preserve as-is"; touching other controllers or `CancelledOrdersController`.
- **Never:** change auth/CORS/password handling; edit repositories, DTOs, entities or migrations for this
  change; open `*.bak`, `data/`, `secrets/`, `log.csv` or `appsettings*.json` secrets; push, merge, or open a PR.

## Success Criteria

1. `OrderingController.cs` is gone. All 52 endpoints in the table answer at the same method + URL.
2. Each endpoint's status code, `Content-Type`, body, and `Pagination` / `Access-Control-Expose-Headers`
   headers match the baseline from the controller (contract tests or diff script pass).
3. Requests that leave out optional int/bool query params still return 200 (bound to 0/false), not 400.
4. `null` repository results still give 204 where they did before.
5. `SetBeingPrepared` / `UnsetBeingPrepared` still broadcast through `OrderHub` to `/moveorder` clients.
6. Swagger shows all 52 operations under "Ordering". The other controllers and `/moveorder` are unaffected.
7. `dotnet build ELIXIR.API/ELIXIR.API.csproj`: 0 errors, no new warnings.

## Decisions (2026-11-07)

1. **Testing:** Option B: new `tests/ELIXIR.Tests` (xUnit, Mvc.Testing, NSubstitute).
2. **Serializer:** keep Newtonsoft for request bodies and responses (assumption 3 confirmed).
3. **Text vs JSON strings:** clients send `Accept: application/json, text/plain, */*`. Because `*/*` is in
   the list, MVC ignores the Accept header and uses `StringOutputFormatter`, so **T** responses stay
   `text/plain` with no quotes. No content negotiation needs to be copied.
4. **Other controllers:** not being converted for some time. `NewtonsoftResults` stays in `ELIXIR.API/Http/`.
5. **Build:** load the matching dotnet skills before building (see the CLAUDE.md skills table).
6. **Auth (from /ship security audit):** the endpoints stay anonymous like the controller. They are not
   controllers, so restoring `[Authorize]` on `BaseApiController` will not cover them. Enable the commented
   `group.RequireAuthorization(...)` line in `OrderingEndpoints.MapOrderingEndpoints` at the same time, or set a global
   `AuthorizationOptions.FallbackPolicy`.
