using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ELIXIR.Tests.Ordering;

/// <summary>
/// Pins the Swagger description of the Ordering endpoints (method, path, tag, parameters, request body and its
/// media types) as captured from the MVC controller, so Swagger stays usable for exercising the API.
/// </summary>
[Trait("Group", "Swagger")]
public sealed class SwaggerContractTests : IClassFixture<ElixirApiFactory>
{
    private readonly ElixirApiFactory _factory;

    public SwaggerContractTests(ElixirApiFactory factory) => _factory = factory;

    /// <summary>Captured from OrderingController on 2026-11-07, in document order.</summary>
    private static readonly string[] Expected =
    {
        "GET /api/Ordering/GetAllListofOrders #Ordering (farms@query:string) body=-",
        "PUT /api/Ordering/EditOrderQuantity #Ordering () body=Ordering[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/SchedulePreparedOrderedDate #Ordering () body=array<Ordering>[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/ApprovePreparedDate #Ordering () body=array<Ordering>[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/RejectPreparedDate #Ordering () body=array<Ordering>[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/GetAllListofPreparedDate #Ordering () body=-",
        "GET /api/Ordering/OrderSummary #Ordering (dateFrom@query:string,dateTo@query:string) body=-",
        "POST /api/Ordering/ValidateNewOrders #Ordering () body=array<Ordering>[application/json-patch+json application/json text/json application/*+json]",
        "POST /api/Ordering/AddNewOrder #Ordering () body=array<Ordering>[application/json-patch+json application/json text/json application/*+json]",
        "GET /api/Ordering/GetAllListOfOrdersPagination #Ordering (PageNumber@query:integer:int32,PageSize@query:integer:int32) body=-",
        "PUT /api/Ordering/CancelOrders #Ordering () body=array<Ordering>[application/json-patch+json application/json text/json application/*+json]",
        "GET /api/Ordering/GetAllListOfCancelledOrders #Ordering () body=-",
        "PUT /api/Ordering/ReturnCancelledOrders #Ordering () body=Ordering[application/json-patch+json application/json text/json application/*+json]",
        "GET /api/Ordering/GetAllListForMoveOrderPagination #Ordering (PageNumber@query:integer:int32,PageSize@query:integer:int32,dateTo@query:string,dateFrom@query:string) body=-",
        "GET /api/Ordering/GetAllListOfApprovedPreparedForMoveOrder #Ordering (farm@query:string) body=-",
        "GET /api/Ordering/DetailedListOfOrders #Ordering (farm@query:string) body=-",
        "GET /api/Ordering/GetAllListForScheduleApproval #Ordering () body=-",
        "GET /api/Ordering/GetAllOrdersForScheduleApproval #Ordering (id@query:integer:int32) body=-",
        "GET /api/Ordering/GetAllOutOfStockByItemCodeAndOrderDate #Ordering (itemcode@query:string,orderdate@query:string) body=-",
        "POST /api/Ordering/PrepareItemsForMoveOrder #Ordering () body=MoveOrder[application/json-patch+json application/json text/json application/*+json]",
        "GET /api/Ordering/GetAllListOfOrdersForMoveOrder #Ordering (id@query:integer:int32) body=-",
        "GET /api/Ordering/GetAvailableStockFromWarehouse #Ordering (id@query:integer:int32,itemcode@query:string) body=-",
        "GET /api/Ordering/ListOfPreparedItemsForMoveOrder #Ordering (id@query:integer:int32) body=-",
        "PUT /api/Ordering/CancelPreparedItems #Ordering () body=MoveOrder[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/AddPlateNumberInMoveOrder #Ordering () body=array<Ordering>[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/AddDeliveryStatus #Ordering () body=array<OrderDto>[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/ApproveListOfMoveOrder #Ordering () body=array<MoveOrder>[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/RejectListOfMoveOrder #Ordering () body=MoveOrder[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/RejectApproveListOfMoveOrder #Ordering () body=MoveOrder[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/ReturnMoveOrderForApproval #Ordering () body=MoveOrder[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/UpdatePrintStatus #Ordering () body=array<integer:int32>[application/json-patch+json application/json text/json application/*+json]",
        "GET /api/Ordering/GetAllForApprovalMoveOrderPagination #Ordering (PageNumber@query:integer:int32,PageSize@query:integer:int32) body=-",
        "GET /api/Ordering/GetAllForApprovalMoveOrderPaginationOrig #Ordering (PageNumber@query:integer:int32,PageSize@query:integer:int32,search@query:string) body=-",
        "GET /api/Ordering/PrintMultipleMoveOrderSlip #Ordering (orderIds@query:array<integer:int32>) body=-",
        "GET /api/Ordering/ViewMoveOrderForApproval #Ordering (id@query:integer:int32) body=-",
        "GET /api/Ordering/ApprovedMoveOrderPagination #Ordering (PageNumber@query:integer:int32,PageSize@query:integer:int32,dateFrom@query:string,dateTo@query:string) body=-",
        "GET /api/Ordering/ApprovedMoveOrderPaginationOrig #Ordering (PageNumber@query:integer:int32,PageSize@query:integer:int32,search@query:string,dateFrom@query:string,dateTo@query:string) body=-",
        "GET /api/Ordering/RejectedMoveOrderPagination #Ordering (PageNumber@query:integer:int32,PageSize@query:integer:int32) body=-",
        "GET /api/Ordering/RejectedMoveOrderPaginationOrig #Ordering (PageNumber@query:integer:int32,PageSize@query:integer:int32,search@query:string) body=-",
        "GET /api/Ordering/GetAllApprovedMoveOrder #Ordering (id@query:integer:int32) body=-",
        "PUT /api/Ordering/CancelOrdersInMoveOrder/{id} #Ordering (id@path:integer:int32!) body=ReasontDTO[application/json-patch+json application/json text/json application/*+json]",
        "GET /api/Ordering/GetAllApprovedOrderCalendar #Ordering () body=-",
        "GET /api/Ordering/GetTotalListForMoveOrder #Ordering (status@query:boolean) body=-",
        "GET /api/Ordering/ListOfMoveOrdersForTransact #Ordering (orderid@query:integer:int32) body=-",
        "POST /api/Ordering/TransactListOfMoveOrders #Ordering () body=array<TransactMoveOrder>[application/json-patch+json application/json text/json application/*+json]",
        "POST /api/Ordering/SetBeingPrepared #Ordering () body=array<Ordering>[application/json-patch+json application/json text/json application/*+json]",
        "POST /api/Ordering/UnsetBeingPrepared #Ordering () body=array<Ordering>[application/json-patch+json application/json text/json application/*+json]",
        "PUT /api/Ordering/Allocate #Ordering () body=AllocationFinalResult[application/json-patch+json application/json text/json application/*+json]",
        "GET /api/Ordering/GetAllListOfOrdersForAllocationPagination #Ordering (PageNumber@query:integer:int32,PageSize@query:integer:int32) body=-",
        "GET /api/Ordering/GetAllListofOrdersForAllocation #Ordering (itemCode@query:string) body=-",
        "POST /api/Ordering/CancelForPendingAllocation #Ordering (customer@query:string) body=-",
        "PUT /api/Ordering/ManualAllocation #Ordering () body=array<ManualAllocation>[application/json-patch+json application/json text/json application/*+json]"
    };

    [Fact]
    public async Task Ordering_operations_match_the_controller_description()
    {
        var json = await _factory.CreateClient().GetStringAsync("swagger/v1/swagger.json");

        var actual = Describe(JObject.Parse(json));

        Assert.Equal(Expected.OrderBy(x => x), actual.OrderBy(x => x));
    }

    private static List<string> Describe(JObject doc)
    {
        var lines = new List<string>();
        foreach (var path in ((JObject)doc["paths"]!).Properties().Where(p => p.Name.StartsWith("/api/Ordering/")))
        {
            foreach (var op in ((JObject)path.Value).Properties())
            {
                var parameters = string.Join(",", (op.Value["parameters"] as JArray ?? new JArray()).Select(p =>
                    $"{p["name"]}@{p["in"]}:{Schema(p["schema"]!)}{(p["required"]?.Value<bool>() == true ? "!" : "")}"));
                var body = "-";
                if (op.Value["requestBody"]?["content"] is JObject content)
                {
                    var first = content.Properties().First();
                    body = Schema(first.Value["schema"]!) + "[" + string.Join(" ", content.Properties().Select(c => c.Name)) + "]";
                }
                var tags = string.Join(",", (op.Value["tags"] as JArray ?? new JArray()).Select(t => (string)t!));
                lines.Add($"{op.Name.ToUpperInvariant()} {path.Name} #{tags} ({parameters}) body={body}");
            }
        }
        return lines;
    }

    private static string Schema(JToken schema)
    {
        if (schema["$ref"] is { } reference) return ((string)reference!).Split('/').Last();
        var type = (string?)schema["type"] ?? "?";
        if (schema["format"] is { } format) type += ":" + (string)format!;
        if (schema["items"] is { } items) type += "<" + Schema(items) + ">";
        return type;
    }
}
