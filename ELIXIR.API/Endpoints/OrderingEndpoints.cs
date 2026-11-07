using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.API.Http;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
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
    }
}
