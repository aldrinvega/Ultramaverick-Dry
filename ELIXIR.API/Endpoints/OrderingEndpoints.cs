using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELIXIR.API.Http;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ELIXIR.API.Endpoints
{
    /// <summary>
    /// Minimal API endpoints for <c>api/Ordering</c> (formerly <c>OrderingController</c>). Routes, binding and responses
    /// are kept identical to the controller: values are bound with <see cref="MvcRequestBinder"/> and responses
    /// are the same MVC results executed through <see cref="MvcResult"/>. See SPEC.md for the contract.
    /// Handler names, parameter names and repository call order match the former controller actions.
    /// </summary>
    public static class OrderingEndpoints
    {
        /// <summary>The former action parameters (name, type, binding source).</summary>
        private static class P
        {
            public static readonly MvcParameter<string> Farms = MvcParameter.Query<string>("farms");
            public static readonly MvcParameter<string> DateFrom = MvcParameter.Query<string>("dateFrom");
            public static readonly MvcParameter<string> DateTo = MvcParameter.Query<string>("dateTo");
            public static readonly MvcParameter<UserParams> UserParams = MvcParameter.Query<UserParams>("userParams");

            public static readonly MvcParameter<Ordering> Order = MvcParameter.Body<Ordering>("order");
            public static readonly MvcParameter<Ordering[]> OrderArray = MvcParameter.Body<Ordering[]>("order");
            public static readonly MvcParameter<List<Ordering>> OrderList = MvcParameter.Body<List<Ordering>>("order");
            public static readonly MvcParameter<Ordering> Orders = MvcParameter.Body<Ordering>("orders");
            public static readonly MvcParameter<Ordering[]> OrdersArray = MvcParameter.Body<Ordering[]>("orders");

            public static readonly MvcParameter<string> Farm = MvcParameter.Query<string>("farm");
            public static readonly MvcParameter<int> Id = MvcParameter.Query<int>("id");
            public static readonly MvcParameter<string> Itemcode = MvcParameter.Query<string>("itemcode");
            public static readonly MvcParameter<string> Orderdate = MvcParameter.Query<string>("orderdate");
            public static readonly MvcParameter<MoveOrder> MoveOrderOrder = MvcParameter.Body<MoveOrder>("order");
            public static readonly MvcParameter<MoveOrder> MoveOrder = MvcParameter.Body<MoveOrder>("moveorder");
            public static readonly MvcParameter<MoveOrder[]> MoveOrderArray = MvcParameter.Body<MoveOrder[]>("moveorder");
            public static readonly MvcParameter<OrderDto[]> OrderDtoArray = MvcParameter.Body<OrderDto[]>("order");
            public static readonly MvcParameter<int[]> PrintIds = MvcParameter.Body<int[]>("moveorder");

            public static readonly MvcParameter<string> Search = MvcParameter.Query<string>("search");
            public static readonly MvcParameter<List<int>> OrderIds = MvcParameter.Query<List<int>>("orderIds");
            public static readonly MvcParameter<ReasontDTO> Reason = MvcParameter.Body<ReasontDTO>("reason");
            public static readonly MvcParameter<int> RouteId = MvcParameter.Route<int>("id");
        }

        public static IEndpointRouteBuilder MapOrderingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/Ordering").WithTags("Ordering");

            group.MapGet("GetAllListofOrders", GetAllListofOrders).WithMvcParameters(P.Farms);
            group.MapPut("EditOrderQuantity", EditOrderQuantity).WithMvcParameters(P.Order);
            group.MapPut("SchedulePreparedOrderedDate", SchedulePreparedOrderedDate).WithMvcParameters(P.OrderArray);
            group.MapPut("ApprovePreparedDate", ApprovePreparedDate).WithMvcParameters(P.OrderList);
            group.MapPut("RejectPreparedDate", RejectPreparedDate).WithMvcParameters(P.OrderList);
            group.MapPut("GetAllListofPreparedDate", GetAllListofPreparedDate);
            group.MapGet("OrderSummary", OrderSummary).WithMvcParameters(P.DateFrom, P.DateTo);
            group.MapPost("ValidateNewOrders", ValidateNewOrders).WithMvcParameters(P.OrderArray);
            group.MapPost("AddNewOrder", AddNewOrdersTest).WithMvcParameters(P.OrderArray);
            group.MapGet("GetAllListOfOrdersPagination", GetAllListOfOrdersPagination).WithMvcParameters(P.UserParams);
            group.MapPut("CancelOrders", CancelOrders).WithMvcParameters(P.OrdersArray);
            group.MapGet("GetAllListOfCancelledOrders", GetAllListOfCancelledOrders);
            group.MapPut("ReturnCancelledOrders", ReturnCancelledOrders).WithMvcParameters(P.Orders);

            group.MapGet("GetAllListForMoveOrderPagination", GetAllListForMoveOrderPagination).WithMvcParameters(P.UserParams, P.DateTo, P.DateFrom);
            group.MapGet("GetAllListOfApprovedPreparedForMoveOrder", GetAllListOfApprovedPreparedForMoveOrder).WithMvcParameters(P.Farm);
            group.MapGet("DetailedListOfOrders", DetailedListOfOrders).WithMvcParameters(P.Farm);
            group.MapGet("GetAllListForScheduleApproval", GetAllListForScheduleApproval);
            group.MapGet("GetAllOrdersForScheduleApproval", GetAllOrdersForScheduleApproval).WithMvcParameters(P.Id);
            group.MapGet("GetAllOutOfStockByItemCodeAndOrderDate", GetAllOutOfStockByItemCodeAndOrderDate).WithMvcParameters(P.Itemcode, P.Orderdate);
            group.MapPost("PrepareItemsForMoveOrder", PrepareItemsForMoveOrder).WithMvcParameters(P.MoveOrderOrder);
            group.MapGet("GetAllListOfOrdersForMoveOrder", GetAllListOfOrdersForMoveOrder).WithMvcParameters(P.Id);
            group.MapGet("GetAvailableStockFromWarehouse", GetAvailableStockFromWarehouse).WithMvcParameters(P.Id, P.Itemcode);
            group.MapGet("ListOfPreparedItemsForMoveOrder", ListOfPreparedItemsForMoveOrder).WithMvcParameters(P.Id);
            group.MapPut("CancelPreparedItems", CancelPreparedItems).WithMvcParameters(P.MoveOrder);
            group.MapPut("AddPlateNumberInMoveOrder", AddPlateNumberInMoveOrder).WithMvcParameters(P.OrderArray);
            group.MapPut("AddDeliveryStatus", AddDeliveryStatus).WithMvcParameters(P.OrderDtoArray);
            group.MapPut("ApproveListOfMoveOrder", ApproveListOfMoveOrder).WithMvcParameters(P.MoveOrderArray);
            group.MapPut("RejectListOfMoveOrder", RejectListOfMoveOrder).WithMvcParameters(P.MoveOrder);
            group.MapPut("RejectApproveListOfMoveOrder", RejectApproveListOfMoveOrder).WithMvcParameters(P.MoveOrder);
            group.MapPut("ReturnMoveOrderForApproval", ReturnMoveOrderForApproval).WithMvcParameters(P.MoveOrder);
            group.MapPut("UpdatePrintStatus", UpdatePrintStatus).WithMvcParameters(P.PrintIds);

            group.MapGet("GetAllForApprovalMoveOrderPagination", GetAllForApprovalMoveOrderPagination).WithMvcParameters(P.UserParams);
            group.MapGet("GetAllForApprovalMoveOrderPaginationOrig", GetAllForApprovalMoveOrderPaginationOrig).WithMvcParameters(P.UserParams, P.Search);
            group.MapGet("PrintMultipleMoveOrderSlip", PrintMultipleMoveOrderSlip).WithMvcParameters(P.OrderIds);
            group.MapGet("ViewMoveOrderForApproval", ViewMoveOrderForApproval).WithMvcParameters(P.Id);
            group.MapGet("ApprovedMoveOrderPagination", ApprovedMoveOrderPagination).WithMvcParameters(P.UserParams, P.DateFrom, P.DateTo);
            group.MapGet("ApprovedMoveOrderPaginationOrig", ApprovedMoveOrderPaginationOrig).WithMvcParameters(P.UserParams, P.Search, P.DateFrom, P.DateTo);
            group.MapGet("RejectedMoveOrderPagination", RejectedMoveOrderPagination).WithMvcParameters(P.UserParams);
            group.MapGet("RejectedMoveOrderPaginationOrig", RejectedMoveOrderPaginationOrig).WithMvcParameters(P.UserParams, P.Search);
            group.MapGet("GetAllApprovedMoveOrder", GetAllApprovedMoveOrder).WithMvcParameters(P.Id);
            group.MapPut("CancelOrdersInMoveOrder/{id}", CancelOrdersInMoveOrder).WithMvcParameters(P.Reason, P.RouteId);
            group.MapGet("GetAllApprovedOrderCalendar", GetAllApprovedOrderCalendar);

            return app;
        }

        private static async Task<IResult> GetAllListofOrders(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var farms = await bind.BindAsync(P.Farms);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.GetAllListOfOrders(farms);
            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> EditOrderQuantity(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var order = await bind.BindAsync(P.Order);
            if (bind.Failure() is { } failure) return failure;

            var result = await unitOfWork.Order.EditQuantityOrder(order);
            if (result == false)
                return MvcResults.BadRequest("Quantity must not transcend the value of allocated quantity!");
            await unitOfWork.CompleteAsync();
            return MvcResults.Json("Successfully edit order quantity!");
        }

        private static async Task<IResult> SchedulePreparedOrderedDate(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var order = await bind.BindAsync(P.OrderArray);
            if (bind.Failure() is { } failure) return failure;

            var generate = new GenerateOrderNo
            {
                IsActive = true
            };

            await unitOfWork.Order.GenerateNumber(generate);
            await unitOfWork.CompleteAsync();

            foreach (Ordering items in order)
            {
                items.OrderNoPKey = generate.Id;

                await unitOfWork.Order.SchedulePreparedDate(items);
            }

            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully scheduled orders");
        }

        private static async Task<IResult> ApprovePreparedDate(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var order = await bind.BindAsync(P.OrderList);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.ApprovePreparedDate(order);
            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully approved prepared date!");
        }

        private static async Task<IResult> RejectPreparedDate(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var order = await bind.BindAsync(P.OrderList);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.RejectPreparedDate(order);
            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully reject prepared date!");
        }

        private static async Task<IResult> GetAllListofPreparedDate([FromServices] IUnitOfWork unitOfWork)
        {
            await unitOfWork.Order.GetAllListOfPreparedDate();
            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully schedules ordered");
        }

        private static async Task<IResult> OrderSummary(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var dateFrom = await bind.BindAsync(P.DateFrom);
            var dateTo = await bind.BindAsync(P.DateTo);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.OrderSummary(dateFrom, dateTo);

            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> ValidateNewOrders(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var order = await bind.BindAsync(P.OrderArray);
            if (bind.Failure() is { } failure) return failure;

            List<Ordering> notExistFarmName = new List<Ordering>();
            List<Ordering> notExistFarmCode = new List<Ordering>();
            List<Ordering> notExistRawMats = new List<Ordering>();
            List<Ordering> notExistUom = new List<Ordering>();
            List<Ordering> duplicateList = new List<Ordering>();
            List<Ordering> previousdateNeeded = new List<Ordering>();
            List<Ordering> filteredOrders = new List<Ordering>();
            List<Ordering> orderList = new List<Ordering>();

            foreach (Ordering items in order)
            {
                var validateDuplicate = await unitOfWork.Order.ValidateExistingOrders(items);
                var validateFarmName = await unitOfWork.Order.ValidateCustomerName(items);
                var validateFarmCode = await unitOfWork.Order.ValidateCustomerCode(items);
                var validateRawMaterial = await unitOfWork.Order.ValidateRawMaterial(items);
                var validateUom = await unitOfWork.Order.ValidateUom(items);
                // var validateDateNeeded = await unitOfWork.Order.ValidateOrderAndDateNeeded(items);

                if (validateDuplicate == false)
                {
                    duplicateList.Add(items);
                }
                else if (validateFarmName == false)
                {
                    notExistFarmName.Add(items);
                }
                else if (validateFarmCode == false)
                {
                    notExistFarmCode.Add(items);
                }
                else if (validateRawMaterial == false)
                {
                    notExistRawMats.Add(items);
                }
                else if (validateUom == false)
                {
                    notExistUom.Add(items);
                }
                // else if (validateDateNeeded == false)
                // {
                //     previousdateNeeded.Add(items);
                // }
                else
                    filteredOrders.Add(items);

                orderList.Add(items);

                await unitOfWork.Order.ValidateNewOrders(items);
            }

            var resultList = new
            {
                duplicateList,
                filteredOrders,
                notExistFarmName,
                notExistFarmCode,
                notExistRawMats,
                notExistUom,
                previousdateNeeded
            };

            if (notExistFarmName.Count == 0 && notExistFarmCode.Count == 0 && notExistRawMats.Count == 0
                && notExistUom.Count == 0 && duplicateList.Count == 0 && previousdateNeeded.Count == 0)
            {
                await unitOfWork.Order.ValidateIfForAllocation(orderList);
                await unitOfWork.CompleteAsync();
                await unitOfWork.CompleteAsync();

                return MvcResults.Ok("Successfully add new orders!");
            }

            return MvcResults.BadRequest(resultList);
        }

        private static async Task<IResult> AddNewOrdersTest(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var order = await bind.BindAsync(P.OrderArray);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.AddNewOrders(order);
            return MvcResults.Ok();
        }

        private static async Task<IResult> GetAllListOfOrdersPagination(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var userParams = await bind.BindAsync(P.UserParams);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.GetAllListofOrdersPagination(userParams);

            // Argument order kept from the controller: the header's hasPrevious/hasNext are swapped (SPEC.md).
            http.Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages,
                orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return MvcResults.Ok(orderResult);
        }

        private static async Task<IResult> CancelOrders(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var orders = await bind.BindAsync(P.OrdersArray);
            if (bind.Failure() is { } failure) return failure;

            var validate = await unitOfWork.Order.CancelOrders(orders);

            if (validate == false)
                return MvcResults.BadRequest("Orders is not exist!");

            await unitOfWork.CompleteAsync();

            return MvcResults.Ok("Successfully cancel orders");
        }

        private static async Task<IResult> GetAllListOfCancelledOrders([FromServices] IUnitOfWork unitOfWork)
        {
            var orders = await unitOfWork.Order.GetAllListOfCancelledOrders();

            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> ReturnCancelledOrders(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var orders = await bind.BindAsync(P.Orders);
            if (bind.Failure() is { } failure) return failure;

            var validate = await unitOfWork.Order.ReturnCancellOrdersInList(orders);

            if (validate == false)
                return MvcResults.BadRequest("Orders is not exist!");

            await unitOfWork.CompleteAsync();

            return MvcResults.Ok("Successfully return orders");
        }

        private static async Task<IResult> GetAllListForMoveOrderPagination(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var userParams = await bind.BindAsync(P.UserParams);
            var dateTo = await bind.BindAsync(P.DateTo);
            var dateFrom = await bind.BindAsync(P.DateFrom);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.GetAllListForMoveOrderPagination(userParams, dateFrom, dateTo);

            http.Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages,
                orders.HasNextPage, orders.HasPreviousPage);

            var orderResult = new
            {
                orders,
                orders.CurrentPage,
                orders.PageSize,
                orders.TotalCount,
                orders.TotalPages,
                orders.HasNextPage,
                orders.HasPreviousPage
            };

            return MvcResults.Ok(orderResult);
        }

        private static async Task<IResult> GetAllListOfApprovedPreparedForMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var farm = await bind.BindAsync(P.Farm);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.TotalListOfApprovedPreparedDate(farm);

            return orders == null ? MvcResults.BadRequest("No Orders Currently Available") : MvcResults.Ok(orders);
        }

        private static async Task<IResult> DetailedListOfOrders(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var farm = await bind.BindAsync(P.Farm);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.DetailedListOfOrders(farm);

            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> GetAllListForScheduleApproval([FromServices] IUnitOfWork unitOfWork)
        {
            var orders = await unitOfWork.Order.GetAllListForApprovalOfSchedule();

            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> GetAllOrdersForScheduleApproval(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var id = await bind.BindAsync(P.Id);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.GetAllOrdersForScheduleApproval(id);

            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> GetAllOutOfStockByItemCodeAndOrderDate(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var itemcode = await bind.BindAsync(P.Itemcode);
            var orderdate = await bind.BindAsync(P.Orderdate);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.GetAllOutOfStockByItemCodeAndOrderDate(itemcode, orderdate);

            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> PrepareItemsForMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var order = await bind.BindAsync(P.MoveOrderOrder);
            if (bind.Failure() is { } failure) return failure;

            var details = await unitOfWork.Order.GetMoveOrderDetailsForMoveOrder(order.OrderNoPKey);

            order.OrderNoPKey = details.Id;
            order.OrderDate = details.OrderDateTime;
            order.DateNeeded = details.DateNeededDateTime;
            order.PreparedDate = details.PreparedDateTime;
            order.FarmName = details.Farm;
            order.FarmCode = details.FarmCode;
            order.FarmType = details.FarmType;
            order.ItemCode = details.ItemCode;
            order.ItemDescription = details.ItemDescription;
            order.Uom = details.Uom;
            order.Category = details.Category;
            order.IsActive = true;
            order.IsPrepared = true;

            await unitOfWork.Order.PrepareItemForMoveOrder(order);
            await unitOfWork.CompleteAsync();

            return MvcResults.Ok(order);
        }

        private static async Task<IResult> GetAllListOfOrdersForMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var id = await bind.BindAsync(P.Id);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.ListOfOrdersForMoveOrder(id);
            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> GetAvailableStockFromWarehouse(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var id = await bind.BindAsync(P.Id);
            var itemcode = await bind.BindAsync(P.Itemcode);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.GetActualItemQuantityInWarehouse(id, itemcode);

            var getFirstExpiry = await unitOfWork.Order.GetFirstExpiry(itemcode);

            var resultList = new
            {
                orders,
                getFirstExpiry.WarehouseId
            };

            return MvcResults.Ok(resultList);
        }

        private static async Task<IResult> ListOfPreparedItemsForMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var id = await bind.BindAsync(P.Id);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.ListOfPreparedItemsForMoveOrder(id);

            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> CancelPreparedItems(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var moveorder = await bind.BindAsync(P.MoveOrder);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.CancelMoveOrder(moveorder);
            await unitOfWork.CompleteAsync();

            return MvcResults.Ok("Successfully cancel prepared items");
        }

        private static async Task<IResult> AddPlateNumberInMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var order = await bind.BindAsync(P.OrderArray);
            if (bind.Failure() is { } failure) return failure;

            foreach (Ordering items in order)
            {
                await unitOfWork.Order.AddPlateNumberInMoveOrder(items);
            }

            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully added plate number!");
        }

        private static async Task<IResult> AddDeliveryStatus(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var order = await bind.BindAsync(P.OrderDtoArray);
            if (bind.Failure() is { } failure) return failure;

            foreach (OrderDto items in order)
            {
                await unitOfWork.Order.AddDeliveryStatus(items);
            }

            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully added delivery status!");
        }

        private static async Task<IResult> ApproveListOfMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var moveorder = await bind.BindAsync(P.MoveOrderArray);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.ApprovalForMoveOrder(moveorder);
            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully approved list for move order!");
        }

        private static async Task<IResult> RejectListOfMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var moveorder = await bind.BindAsync(P.MoveOrder);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.RejectForMoveOrder(moveorder);
            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully reject list for move order!");
        }

        private static async Task<IResult> RejectApproveListOfMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var moveorder = await bind.BindAsync(P.MoveOrder);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.RejectApproveMoveOrder(moveorder);
            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully reject approved list for move order!");
        }

        private static async Task<IResult> ReturnMoveOrderForApproval(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var moveorder = await bind.BindAsync(P.MoveOrder);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.ReturnMoveOrderForApproval(moveorder);
            await unitOfWork.CompleteAsync();

            return MvcResults.Json("Successfully return list for move order!");
        }

        private static async Task<IResult> UpdatePrintStatus(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var moveorder = await bind.BindAsync(P.PrintIds);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.UpdatePrintStatus(moveorder);
            return MvcResults.Ok(moveorder);
        }

        /// <summary>The paginated move-order response shared by the approval/approved/rejected list endpoints.</summary>
        private static IResult MoveOrderPage(HttpContext http, PagedList<MoveOrderDto> moveorder)
        {
            http.Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount,
                moveorder.TotalPages, moveorder.HasNextPage, moveorder.HasPreviousPage);

            var moveorderResult = new
            {
                moveorder,
                moveorder.CurrentPage,
                moveorder.PageSize,
                moveorder.TotalCount,
                moveorder.TotalPages,
                moveorder.HasNextPage,
                moveorder.HasPreviousPage
            };

            return MvcResults.Ok(moveorderResult);
        }

        private static async Task<IResult> GetAllForApprovalMoveOrderPagination(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var userParams = await bind.BindAsync(P.UserParams);
            if (bind.Failure() is { } failure) return failure;

            return MoveOrderPage(http, await unitOfWork.Order.ForApprovalMoveOrderPagination(userParams));
        }

        private static async Task<IResult> GetAllForApprovalMoveOrderPaginationOrig(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var userParams = await bind.BindAsync(P.UserParams);
            var search = await bind.BindAsync(P.Search);
            if (bind.Failure() is { } failure) return failure;

            if (search == null)
                return MoveOrderPage(http, await unitOfWork.Order.ForApprovalMoveOrderPagination(userParams));

            return MoveOrderPage(http, await unitOfWork.Order.ForApprovalMoveOrderPaginationOrig(userParams, search));
        }

        private static async Task<IResult> PrintMultipleMoveOrderSlip(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var orderIds = await bind.BindAsync(P.OrderIds);
            if (bind.Failure() is { } failure) return failure;

            if (orderIds == null || !orderIds.Any())
                return MvcResults.BadRequest("No order IDs provided");

            var orders = await unitOfWork.Order.MultiplePrintingForMOS(orderIds);
            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> ViewMoveOrderForApproval(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var id = await bind.BindAsync(P.Id);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.ViewMoveOrderForApproval(id);

            if (orders.Count != 0)
            {
                return MvcResults.Ok(orders);
            }

            var order = await unitOfWork.Order.ViewMoveOrderForApprovalOriginal(id);
            return MvcResults.Ok(order);
        }

        private static async Task<IResult> ApprovedMoveOrderPagination(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var userParams = await bind.BindAsync(P.UserParams);
            var dateFrom = await bind.BindAsync(P.DateFrom);
            var dateTo = await bind.BindAsync(P.DateTo);
            if (bind.Failure() is { } failure) return failure;

            return MoveOrderPage(http, await unitOfWork.Order.ApprovedMoveOrderPagination(userParams, dateFrom, dateTo));
        }

        private static async Task<IResult> ApprovedMoveOrderPaginationOrig(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var userParams = await bind.BindAsync(P.UserParams);
            var search = await bind.BindAsync(P.Search);
            var dateFrom = await bind.BindAsync(P.DateFrom);
            var dateTo = await bind.BindAsync(P.DateTo);
            if (bind.Failure() is { } failure) return failure;

            if (search == null)
                return MoveOrderPage(http, await unitOfWork.Order.ApprovedMoveOrderPagination(userParams, dateFrom, dateTo));

            return MoveOrderPage(http,
                await unitOfWork.Order.ApprovedMoveOrderPaginationOrig(userParams, search, dateFrom, dateTo));
        }

        private static async Task<IResult> RejectedMoveOrderPagination(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var userParams = await bind.BindAsync(P.UserParams);
            if (bind.Failure() is { } failure) return failure;

            return MoveOrderPage(http, await unitOfWork.Order.RejectedMoveOrderPagination(userParams));
        }

        private static async Task<IResult> RejectedMoveOrderPaginationOrig(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var userParams = await bind.BindAsync(P.UserParams);
            var search = await bind.BindAsync(P.Search);
            if (bind.Failure() is { } failure) return failure;

            if (search == null)
                return MoveOrderPage(http, await unitOfWork.Order.RejectedMoveOrderPagination(userParams));

            return MoveOrderPage(http, await unitOfWork.Order.RejectedMoveOrderPaginationOrig(userParams, search));
        }

        private static async Task<IResult> GetAllApprovedMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var id = await bind.BindAsync(P.Id);
            if (bind.Failure() is { } failure) return failure;

            var orders = await unitOfWork.Order.GetAllApprovedMoveOrder(id);

            return MvcResults.Ok(orders);
        }

        private static async Task<IResult> CancelOrdersInMoveOrder(HttpContext http, [FromServices] IUnitOfWork unitOfWork)
        {
            var bind = await MvcRequestBinder.CreateAsync(http);
            var reason = await bind.BindAsync(P.Reason);
            var id = await bind.BindAsync(P.RouteId);
            if (bind.Failure() is { } failure) return failure;

            await unitOfWork.Order.CancelControlInMoveOrder(id, reason);

            await unitOfWork.CompleteAsync();

            return MvcResults.Ok("Successfully cancel orders");
        }

        private static async Task<IResult> GetAllApprovedOrderCalendar([FromServices] IUnitOfWork unitOfWork)
        {
            var orders = await unitOfWork.Order.GetAllApprovedOrdersForCalendar();

            return MvcResults.Ok(orders);
        }
    }
}
