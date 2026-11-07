using System;
using System.Collections.Generic;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using OrderingEntity = ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL.Ordering;

namespace ELIXIR.Tests.Ordering;

/// <summary>Deterministic sample payloads. Dates/decimals are chosen to expose serializer differences.</summary>
internal static class TestData
{
    public static OrderingEntity Order(int id = 1) => new()
    {
        Id = id,
        CustomerName = "Farm A",
        FarmCode = "F01",
        FarmName = "Farm A",
        FarmType = "Poultry",
        OrderNo = 100 + id,
        OrderDate = new DateTime(2024, 1, 2),
        DateNeeded = new DateTime(2024, 1, 5),
        ItemCode = "RM-001",
        ItemDescription = "Corn",
        Uom = "KG",
        QuantityOrdered = 10.5m,
        Category = "Feeds",
        IsActive = true,
        PreparedDate = new DateTime(2024, 1, 3, 8, 30, 0)
    };

    public static OrderDto OrderDto(int id = 1) => new()
    {
        Id = id,
        OrderNo = 200 + id,
        OrderDate = "01/02/2024",
        DateNeeded = "01/05/2024",
        Farm = "Farm A",
        FarmCode = "F01",
        FarmType = "Poultry",
        Category = "Feeds",
        ItemCode = "RM-001",
        ItemDescription = "Corn",
        Uom = "KG",
        QuantityOrder = 12.25m,
        OrderDateTime = new DateTime(2024, 1, 2, 3, 4, 5),
        DateNeededDateTime = new DateTime(2024, 1, 5),
        PreparedDateTime = null,
        IsBeingPrepared = true
    };

    public static MoveOrder MoveOrder(int id = 1) => new()
    {
        Id = id,
        OrderNo = 300 + id,
        OrderNoPKey = 40 + id,
        FarmName = "Farm A",
        FarmCode = "F01",
        ItemCode = "RM-001",
        Uom = "KG",
        QuantityOrdered = 7.75m,
        OrderDate = new DateTime(2024, 1, 2),
        DateNeeded = new DateTime(2024, 1, 5)
    };

    public static MoveOrderDto MoveOrderDto(int id = 1) => new()
    {
        Id = id,
        OrderNo = 400 + id,
        FarmCode = "F01",
        FarmName = "Farm A",
        FarmType = "Poultry",
        Category = "Feeds",
        Uom = "KG",
        OrderDate = "01/02/2024",
        DateNeeded = "01/05/2024"
    };

    /// <summary>
    /// Page 3 of 3 with page size 1 (header: <see cref="Contract.LastPageHeader"/>). HasNextPage and HasPreviousPage
    /// differ, so the swapped AddPaginationHeader arguments are observable.
    /// </summary>
    public static PagedList<T> LastPage<T>(T item) => new(new List<T> { item }, count: 3, pageNumber: 3, pageSize: 1);

    /// <summary>Page 1 of 3 with page size 1 (header: <see cref="Contract.PageOneHeader"/>).</summary>
    public static PagedList<T> PageOne<T>(T item) => new(new List<T> { item }, count: 3, pageNumber: 1, pageSize: 1);
}
