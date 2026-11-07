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