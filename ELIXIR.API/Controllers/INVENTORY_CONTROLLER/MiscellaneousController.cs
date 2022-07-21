using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.INVENTORY_CONTROLLER
{


    [ApiController]
    public class MiscellaneousController : BaseApiController
    {

        private readonly IUnitOfWork _unitOfWork;

        public MiscellaneousController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }

        [HttpPost]
        [Route("AddNewMiscellaneousReceipt")]
        public async Task<IActionResult> AddNewMiscellaneousReceipt([FromBody] MiscellaneousReceipt[] receipt)
        {
            DateTime dateNow = DateTime.Now;


            var generate = new GenerateMReceipt();
            var warehouse = new WarehouseReceiving();

            generate.IsActive = true;

            await _unitOfWork.Miscellaneous.GenerateReceiptNumber(generate);
            await _unitOfWork.CompleteAsync();

            foreach (MiscellaneousReceipt items in receipt)
            {

                items.ReceiptPKey = generate.Id;
                items.PreparedDate = DateTime.Now;
                items.IsActive = true;

                await _unitOfWork.Miscellaneous.AddMiscellaneousReceipt(items);

                warehouse.ItemCode = items.ItemCode;
                warehouse.ItemDescription = items.ItemDescription;
                warehouse.Uom = items.Uom;
                warehouse.Supplier = items.Supplier;
                warehouse.Expiration = items.ExpirationDate;
                warehouse.ExpirationDays = items.ExpirationDate.Subtract(dateNow).Days;
                warehouse.ActualGood = items.Quantity;
                warehouse.ReceivingDate = DateTime.Now;
                warehouse.IsActive = true;
                warehouse.IsWarehouseReceive = true;
                warehouse.TransactionType = "MiscelleneousReceipt";
                warehouse.ReceivedBy = items.PreparedBy;
                warehouse.MiscellaneousReceiptId = items.ReceiptPKey;

                await _unitOfWork.Miscellaneous.AddWarehouseReceiveForReceipt(warehouse);
            }

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully add new miscellaneous receipt!");

        }

        [HttpPost]
        [Route("AddNewMiscellaneousIssue")]
        public async Task<IActionResult> AddNewMiscellaneousIssue([FromBody] MiscellaneousIssue[] issue)
        {

            var generate = new GenerateMIssue();

            await _unitOfWork.Miscellaneous.GenerateIssueNumber(generate);
            await _unitOfWork.CompleteAsync();

            foreach (MiscellaneousIssue items in issue)
            {
                items.IssuePKey = generate.Id;
                await _unitOfWork.Miscellaneous.AddMiscellaneousIssue(items);
            }

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully add new miscellaneous issue!");

        }

    }
}
