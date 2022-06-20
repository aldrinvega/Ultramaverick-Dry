using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.ORDERING_CONTROLLER
{
    [ApiController]
    public class OrderingController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderingController(IUnitOfWork unitofwork)
        {

            _unitOfWork = unitofwork;

        }

        [HttpGet]
        [Route("GetAllListofOrders")]
        public async Task<IActionResult> GetAllListofOrders([FromQuery] string farms)
        {

            var orders = await _unitOfWork.Order.GetAllListofOrders(farms);

            return Ok(orders);

        }

        [HttpPut]
        [Route("EditOrderQuantity")]
        public async Task<IActionResult> EditOrderQuantity([FromBody] Ordering order)
        {

            await _unitOfWork.Order.EditQuantityOrder(order);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully edit order quantity!");
        }

        [HttpPut]
        [Route("SchedulePreparedOrderedDate")]
        public async Task<IActionResult> SchedulePreparedOrderedDate([FromBody] Ordering[] order)
        {


            foreach(Ordering items in order)
            {
                await _unitOfWork.Order.SchedulePreparedDate(items);
                
            }

            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully scheduled orders");
        }

        [HttpPut]
        [Route("ApprovePreparedDate")]
        public async Task<IActionResult> ApprovePreparedDate([FromBody] Ordering order)
        {

            await _unitOfWork.Order.ApprovePreparedDate(order);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully approved prepared date!");
        }

        [HttpPut]
        [Route("RejectPreparedDate")]
        public async Task<IActionResult> RejectPreparedDate([FromBody] Ordering order)
        {

            await _unitOfWork.Order.RejectPreparedDate(order);
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully reject prepared date!");
        }

        [HttpPut]
        [Route("GetAllListofPreparedDate")]
        public async Task<IActionResult> GetAllListofPreparedDate()
        {

            await _unitOfWork.Order.GetAllListOfPreparedDate();
            await _unitOfWork.CompleteAsync();

            return new JsonResult("Successfully schedules ordered");
        }


        [HttpGet]
        [Route("OrderSummary")]
        public async Task<IActionResult> OrderSummary([FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            var orders = await _unitOfWork.Order.OrderSummary(DateFrom, DateTo);

            return Ok(orders);

        }


        [HttpPost]
        [Route("AddNewOrders")]
        public async Task<IActionResult> AddNewOrders([FromBody]Ordering[] order)
        {

            List<Ordering> notExistFarmName = new List<Ordering>();
            List<Ordering> notExistFarmCode = new List<Ordering>();
            List<Ordering> notExistRawMats = new List<Ordering>();
            List<Ordering> notExistUom = new List<Ordering>();
            List<Ordering> duplicateList = new List<Ordering>();
         //   List<Ordering> invaliddatelist = new List<Ordering>();


            List<Ordering> filteredOrders = new List<Ordering>();


            foreach (Ordering items in order)
            {

                var validateDuplicate = await _unitOfWork.Order.ValidateExistingOrders(items);
                var validateFarmName = await _unitOfWork.Order.ValidateFarmType(items);
                var validateFarmCode = await _unitOfWork.Order.ValidateFarmCode(items);
                var validateRawMaterial = await _unitOfWork.Order.ValidateRawMaterial(items);
                var validateUom = await _unitOfWork.Order.ValidateUom(items);
             //   var validateDate = await _unitOfWork.Order.ValidateOrderAndDateNeeded(items);


                if (validateDuplicate == false)
                {
                    duplicateList.Add(items);
                }

                //else if (validateDate == false)
                //{
                //    invaliddatelist.Add(items);
                //}

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

                else
                    filteredOrders.Add(items);


                await _unitOfWork.Order.AddNewOrders(items);
            }


            var resultList = new
            {
                duplicateList,
                filteredOrders,
              //  invaliddatelist,
                notExistFarmName,
                notExistFarmCode,
                notExistRawMats,
                notExistUom,

            };

            if (notExistFarmName.Count == 0 && notExistFarmCode.Count == 0 && notExistRawMats.Count == 0 
                                    && notExistUom.Count == 0 && duplicateList.Count == 0)
            {
                await _unitOfWork.CompleteAsync();

                return Ok("Successfully add new orders!");
            }

            else
            {
                return BadRequest(resultList);
            }
         
        }

        [HttpGet]
        [Route("GetAllListOfOrdersPagination")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllListOfOrdersPagination([FromQuery] UserParams userParams)
        {

            var orders = await _unitOfWork.Order.GetAllListofOrdersPagination(userParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

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


        [HttpPut]
        [Route("CancelOrders")]
        public async Task<IActionResult> CancelOrders([FromBody] Ordering orders)
        {

            var validate = await _unitOfWork.Order.CancelOrders(orders);

            if (validate == false)
                return BadRequest("Orders is not exist!");

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully cancel orders");

        }

        [HttpGet]
        [Route("GetAllListOfCancelledOrders")]
        public async Task<IActionResult> GetAllListOfCancelledOrders()
        {

            var orders = await _unitOfWork.Order.GetAllListOfCancelledOrders();

            return Ok(orders);

        }

        [HttpPut]
        [Route("ReturnCancelledOrders")]
        public async Task<IActionResult> ReturnCancelledOrders([FromBody] Ordering orders)
        {

            var validate = await _unitOfWork.Order.ReturnCancellOrdersInList(orders);

            if (validate == false)
                return BadRequest("Orders is not exist!");

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully return orders");

        }

        [HttpGet]
        [Route("GetAllListForMoveOrderPagination")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllListForMoveOrderPagination([FromQuery] UserParams userParams)
        {

            var orders = await _unitOfWork.Order.GetAllListForMoveOrderPagination(userParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages, orders.HasNextPage, orders.HasPreviousPage);

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

        [HttpGet]
        [Route("GetAllListOfApprovedPreparedForMoveOrder")]
        public async Task<IActionResult> GetAllListOfApprovedPreparedForMoveOrder([FromQuery] string farm)
        {

            var orders = await _unitOfWork.Order.GetAllListOfApprovedPreparedDate(farm);

            return Ok(orders);

        }

    }
}
