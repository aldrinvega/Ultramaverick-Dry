using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.API.Controllers.CANCELLEDORDERES_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelledOrdersController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public CancelledOrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(CancelledOrders cancelledOrder)
        {

            var result = await _unitOfWork.CancelledOrders.VoidOrder(cancelledOrder);
            if (result)
            {
                await _unitOfWork.CompleteAsync();
                return Ok($"Order has been is cancelled");
            }

            return BadRequest();
        }
        //[HttpGet("GetCancelledOrders/{customerId}")]
        //public async Task<IActionResult> GetCancelledOrdersByCustomer(int customerId)
        //{
        //    var result = await _unitOfWork.CancelledOrders.GetAllCancelledOrdersByCustomer(customerId);
        //    return Ok(result);
        //}

        //[HttpGet("GetAllOrderandcancelledOrders")]
        //public async Task<IActionResult> GetAllOrderandcancelledOrders()
        //{
        //    var result = await _unitOfWork.CancelledOrders.GetAllOrderandcancelledOrders();
        //    return Ok(result);
        //}

        [HttpGet("GetCancelledOrdersById/{customerId}")]
        public async Task<IActionResult> GetCancelledOrdersById(int customerId)
        {

            var result = await _unitOfWork.CancelledOrders.GetCancelledOrdersById(customerId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllListOfCancelledOrdersPagination")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllListOfCancelledOrdersPagination(
           [FromQuery] UserParams userParams)
        {

            var customers = await _unitOfWork.CancelledOrders.GetAllcancelledOrdersPagination(userParams);

            Response.AddPaginationHeader(customers.CurrentPage, customers.PageSize, customers.TotalCount, customers.TotalPages,
                customers.HasNextPage, customers.HasPreviousPage);

            var cancelledOrdersResult = new
            {
                customers,
                customers.CurrentPage,
                customers.PageSize,
                customers.TotalCount,
                customers.TotalPages,
                customers.HasNextPage,
                customers.HasPreviousPage
            };

            return Ok(cancelledOrdersResult);
        }
    }
}
