using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.ORDER_HUB;
using ELIXIR.DATA.SERVICES;
using Microsoft.AspNetCore.SignalR;

namespace ELIXIR.API.Controllers.ORDERING_CONTROLLER
{
    [ApiController]
    public class OrderingController : BaseApiController
    {
        private readonly IHubContext<OrderHub, IOrderHub> _hub;
        private readonly IUnitOfWork _unitOfWork;

        public OrderingController(IUnitOfWork unitofwork, IHubContext<OrderHub, IOrderHub> hub)
        {
            _unitOfWork = unitofwork;
            _hub = hub;
        }

        [HttpGet]
        [Route("GetAllForApprovalMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> GetAllForApprovalMoveOrderPagination(
            [FromQuery] UserParams userParams)
        {
            var moveorder = await _unitOfWork.Order.ForApprovalMoveOrderPagination(userParams);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount,
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

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("GetAllForApprovalMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> GetAllForApprovalMoveOrderPaginationOrig(
            [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)

                return await GetAllForApprovalMoveOrderPagination(userParams);

            var moveorder = await _unitOfWork.Order.ForApprovalMoveOrderPaginationOrig(userParams, search);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount,
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

            return Ok(moveorderResult);
        }


        [HttpGet]
        [Route("PrintMultipleMoveOrderSlip")]
        public async Task<IActionResult> PrintMultipleMoveOrderSlip([FromQuery] List<int> orderIds)
        {
            if (orderIds == null || !orderIds.Any())
                return BadRequest("No order IDs provided");

            var orders = await _unitOfWork.Order.MultiplePrintingForMOS(orderIds);
            return Ok(orders);
        }

        [HttpGet]
        [Route("ViewMoveOrderForApproval")]
        public async Task<IActionResult> ViewMoveOrderForApproval([FromQuery] int id)
        {
            var orders = await _unitOfWork.Order.ViewMoveOrderForApproval(id);

            if (orders.Count != 0)
            {
                return Ok(orders);
            }

            var order = await _unitOfWork.Order.ViewMoveOrderForApprovalOriginal(id);
            return Ok(order);
        }

        [HttpGet]
        [Route("ApprovedMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> ApprovedMoveOrderPagination(
            [FromQuery] UserParams userParams, string dateFrom, string dateTo)
        {
            var moveorder = await _unitOfWork.Order.ApprovedMoveOrderPagination(userParams, dateFrom, dateTo);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount,
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

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("ApprovedMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> ApprovedMoveOrderPaginationOrig(
            [FromQuery] UserParams userParams, [FromQuery] string search, string dateFrom, string dateTo)
        {
            if (search == null)

                return await ApprovedMoveOrderPagination(userParams, dateFrom, dateTo);

            var moveorder =
                await _unitOfWork.Order.ApprovedMoveOrderPaginationOrig(userParams, search, dateFrom, dateTo);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount,
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

            return Ok(moveorderResult);
        }


        [HttpGet]
        [Route("RejectedMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> RejectedMoveOrderPagination(
            [FromQuery] UserParams userParams)
        {
            var moveorder = await _unitOfWork.Order.RejectedMoveOrderPagination(userParams);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount,
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

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("RejectedMoveOrderPaginationOrig")]
        public async Task<ActionResult<IEnumerable<MoveOrderDto>>> RejectedMoveOrderPaginationOrig(
            [FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)

                return await RejectedMoveOrderPagination(userParams);

            var moveorder = await _unitOfWork.Order.RejectedMoveOrderPaginationOrig(userParams, search);

            Response.AddPaginationHeader(moveorder.CurrentPage, moveorder.PageSize, moveorder.TotalCount,
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

            return Ok(moveorderResult);
        }

        [HttpGet]
        [Route("GetAllApprovedMoveOrder")]
        public async Task<IActionResult> GetAllApprovedMoveOrder([FromQuery] int id)
        {
            var orders = await _unitOfWork.Order.GetAllApprovedMoveOrder(id);

            return Ok(orders);
        }

        [HttpPut]
        [Route("CancelOrdersInMoveOrder/{id}")]
        public async Task<IActionResult> CancelOrdersInMoveOrder([FromBody] ReasontDTO reason, [FromRoute]int id)
        {
            await _unitOfWork.Order.CancelControlInMoveOrder(id, reason);

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully cancel orders");
        }

        [HttpGet]
        [Route("GetAllApprovedOrderCalendar")]
        public async Task<IActionResult> GetAllApprovedOrderCalendar()
        {
            var orders = await _unitOfWork.Order.GetAllApprovedOrdersForCalendar();

            return Ok(orders);
        }

        //------------------TRANSACT MOVE ORDER-------------------------

        [HttpGet]
        [Route("GetTotalListForMoveOrder")]
        public async Task<IActionResult> GetTotalListForMoveOrder([FromQuery] bool status)
        {
            var orders = await _unitOfWork.Order.TotalListForTransactMoveOrder(status);

            return Ok(orders);
        }

        [HttpGet]
        [Route("ListOfMoveOrdersForTransact")]
        public async Task<IActionResult> ListOfMoveOrdersForTransact([FromQuery] int orderid)
        {
            var orders = await _unitOfWork.Order.ListOfMoveOrdersForTransact(orderid);

            return Ok(orders);
        }

        [HttpPost]
        [Route("TransactListOfMoveOrders")]
        public async Task<IActionResult> TransactListOfMoveOrders([FromBody] TransactMoveOrder[] transact)
        {
            foreach (TransactMoveOrder items in transact)
            {
                items.IsActive = true;
                items.IsTransact = true;
                items.PreparedDate = DateTime.Now;

                await _unitOfWork.Order.TransanctListOfMoveOrders(items);
            }

            await _unitOfWork.CompleteAsync();

            return Ok(transact);
        }

        ////NEW METHODS////

        [HttpPost]
        [Route("SetBeingPrepared")]
        public async Task<IActionResult> SetBeingPrepared([FromBody] Ordering[] moveOrders)
        {
            foreach (var item in moveOrders)
            {
                var result = await _unitOfWork.Order.SetBeingPrepared(item);

                if (!result) continue;
                // Call the SignalR hub method to broadcast the update
                await _hub.Clients.All.SetBeingPrepared(item);
                return Ok();
            }

            return new JsonResult("Someone is already preparing!");
        }

        [HttpPost]
        [Route("UnsetBeingPrepared")]
        public async Task<IActionResult> UnsetBeingPrepared([FromBody] Ordering[] orderNos)
        {
            foreach (Ordering item in orderNos)
            {
                var result = await _unitOfWork.Order.UnsetBeingPrepared(item);

                if (result != null)
                {
                    await _hub.Clients.All.SetBeingPrepared(item);
                    return Ok();
                }
            }

            return new JsonResult("Something Wrong");
        }

        //========Allocation========//

        [HttpPut("Allocate")]
        public async Task<IActionResult> AllocateOrderPerItem([FromBody] AllocationFinalResult allocation)
        {
            var orders = await _unitOfWork.Order.AllocateOrdersPerItems(allocation);
            await _unitOfWork.CompleteAsync();
            return Ok(orders);
        }

        [HttpGet]
        [Route("GetAllListOfOrdersForAllocationPagination")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllListOfOrdersForAllocationPagination(
            [FromQuery] UserParams userParams)
        {
            var orders = await _unitOfWork.Order.GetAllListofOrdersForAllocationPagination(userParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages,
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

            return Ok(orderResult);
        }

        [HttpGet("GetAllListofOrdersForAllocation")]
        public async Task<IActionResult> GetAllListofOrdersForAllocation([FromQuery] string itemCode)
        {
            var orders = await _unitOfWork.Order.GetAllListofOrdersAllocation(itemCode);

            return Ok(orders);
        }

        [HttpPost("CancelForPendingAllocation")]
        public async Task<ActionResult> CancelForAllocation([FromQuery] string customer)
        {
            var order = await _unitOfWork.Order.CancelForPendingAllocation(customer);

            if (!order)
                return BadRequest("Something went wrong");

            return Ok();
        }

        //Checklist

        //[HttpGet("GetAllChecklistByPoSummaryId")]

        //public async Task<IActionResult> GetAllChecklistByPoSummaryId()
        //{
        //    var checklist = await _unitOfWork.QcChecklist.GetAllChecklist();

        //    if (checklist == null)
        //        return BadRequest("No Checklist Found");
        //    return Ok(checklist);
        //}

        [HttpPut("ManualAllocation")]
        public async Task<IActionResult> ManualAllocateOrders(List<ManualAllocation> order)
        {
            var isSuccess = await _unitOfWork.Order.ManualAllocationForOrders(order);
            await _unitOfWork.CompleteAsync();
            if (isSuccess)
                return Ok("Manual Allocation Complete");

            return new JsonResult("Something Wrong");
        }

        /*[HttpPost("AddChecklist")]
        public async Task<IActionResult> AddChecklist(Checklists input)
        {
            await _unitOfWork.QcChecklist.AddChecklists(input);
            return Ok("Goods");
        }*/
    }
}