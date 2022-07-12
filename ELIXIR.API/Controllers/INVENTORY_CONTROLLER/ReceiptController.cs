using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.INVENTORY_CONTROLLER
{

    [ApiController]
    public class ReceiptController : BaseApiController
    {

        private readonly IUnitOfWork _unitOfWork;

        public ReceiptController(IUnitOfWork unitofwork)
        {

            _unitOfWork = unitofwork;

        }


        [HttpPost]
        [Route("AddNewMiscellaneousReceipt")]
        public async Task<IActionResult> AddNewMiscellaneousReceipt ([FromBody] MiscellaneousReceipt[] receipt)
        {

            var generate = new GenerateMReceipt();

            generate.IsActive = true;

            await _unitOfWork.Miscellaneous.GenerateReceiptNumber(generate);
            await _unitOfWork.CompleteAsync();

            foreach(MiscellaneousReceipt items in receipt)
            {

                items.ReceiptPKey = generate.Id;

                await _unitOfWork.Miscellaneous.AddMiscellaneousReceipt(items);

            }

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully add new miscellaneous receipt!");

        }


    }
}
