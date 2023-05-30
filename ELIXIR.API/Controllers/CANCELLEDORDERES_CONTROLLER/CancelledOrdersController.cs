using System.Threading.Tasks;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
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

        [HttpGet("GetAllOrderandcancelledOrdersById/{customerId}")]
        public async Task<IActionResult> GetAllOrderandcancelledOrdersById(int customerId)
        {

            var result = await _unitOfWork.CancelledOrders.GetAllOrderandcancelledOrdersById(customerId);
            return Ok(result);
        }
    }
}
