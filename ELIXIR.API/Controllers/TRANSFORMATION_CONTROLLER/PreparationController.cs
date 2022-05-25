using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.TRANSFORMATION_CONTROLLER
{
  
    public class PreparationController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PreparationController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }


        [HttpGet]
        [Route("GetAllRequirementsByTransformId")]
        public async Task<IActionResult> GetAllRequirementsByTransformId([FromQuery] TransformationPlanning planning)
        {

            var requirement = await _unitOfWork.Preparation.GetAllListOfTransformationByTransformationId(planning);

            return Ok(requirement);

        }

        [HttpPut]
        [Route("PrepareMaterialsForRequest/{id}")]
        public async Task<IActionResult> PrepareMaterialsForRequest(int id,  [FromBody] TransformationPreparation preparation)
        {

            List<ItemStocks> AddList = new List<ItemStocks>();

            decimal quantity = 0;
            decimal tempquantity = 0;
            decimal tempWeighingScale = 0;
            decimal tempBalance = 0;

            if (id != preparation.TransformId)
                return BadRequest();


            var validate =  await _unitOfWork.Preparation.ValidatePreparedMaterials(preparation.TransformId, preparation.ItemCode);

            if (validate == false)
                return BadRequest("Already prepared this material!");

            var validateApproved = await _unitOfWork.Preparation.ValidateIfApproved(preparation.TransformId);

            if (validateApproved == false)
                return BadRequest("Preparing material failed, please check if the request is approved");


            var getRemainingStocks = await _unitOfWork.Preparation.GetAllRemainingStocksPerReceivingId(preparation.ItemCode);


            foreach(var items in getRemainingStocks)
            {
                quantity = quantity + tempWeighingScale;
         //       preparation.WeighingScale = 0;

                if(quantity != preparation.WeighingScale)
                {
                     
                    preparation.Id = 0;

                    if (items.Remaining < preparation.WeighingScale)
                    {

                        tempquantity = items.Remaining;
                        tempWeighingScale = preparation.WeighingScale;

             
                        preparation.WeighingScale = tempquantity;
                        preparation.WarehouseId = items.WarehouseId;
                        preparation.ItemCode = items.ItemCode;
                        preparation.IsActive = true;
                        preparation.PreparedDate = DateTime.Now;
                        
                        await _unitOfWork.Preparation.PrepareTransformationMaterials(preparation);


                        tempquantity = tempWeighingScale - items.Remaining;


                    }

                    else if (items.Remaining > preparation.WeighingScale)
                    {

                        //tempquantity = items.Remaining;

                        //preparation.WeighingScale = tempquantity;
                        //preparation.WarehouseId = items.WarehouseId;
                        //preparation.ItemCode = items.ItemCode;
                        //preparation.IsActive = true;
                        //preparation.PreparedDate = DateTime.Now;

                        await _unitOfWork.Preparation.PrepareTransformationMaterials(preparation);
                        await _unitOfWork.CompleteAsync();
                        return Ok("Successfully prepared materials!");

                    }

                } 
            }

            if (quantity != preparation.WeighingScale)
                return BadRequest("Prepared failed, not enough stocks!");


       //     var validatematerial = await _unitOfWork.Preparation.PrepareTransformationMaterials(preparation);

            //if (validatematerial == false)
            //    return BadRequest("Prepared failed, please check your input in weighing scale!");


            //await _unitOfWork.CompleteAsync();



            return Ok(preparation);

        }

        [HttpGet]
        [Route("GetAllRequestForMixing")]
        public async Task<IActionResult> GetAllRequestForMixing()
        {

            var mixing = await _unitOfWork.Preparation.GetAllListOfTransformationRequestForMixing();

            return Ok(mixing);

        }

        [HttpGet]
        [Route("GetAllRequirementsForMixing")]
        public async Task<IActionResult> GetAllRequirementsForMixing([FromQuery]int id)
        {

            var requirement = await _unitOfWork.Preparation.GetAllRequirementsForMixing(id);

            return Ok(requirement);

        }


        [HttpPut]
        [Route("FinishedMixedMaterialsForWarehouse/{id}")]
        public async Task<IActionResult> FinishedMixedMaterialsForWarehouse(int id, [FromBody] WarehouseReceiving warehouse)
        {

            //if (id != warehouse.TransformId)
            //    return BadRequest();


           var validate = await _unitOfWork.Preparation.FinishedMixedMaterialsForWarehouse(warehouse);

            if (validate == false)
                return BadRequest("Already mixed all batch for this request!");

            await _unitOfWork.CompleteAsync();

            return Ok(warehouse);

        }


        [HttpGet]
        [Route("GetRawmaterialDetailsInWarehouse")]
        public async Task<IActionResult> GetRawmaterialDetailsInWarehouse([FromQuery] string code)
        {

            var requirement = await _unitOfWork.Preparation.GetReceivingDetailsForRawmaterials(code);

            return Ok(requirement);

        }


        [HttpGet]
        [Route("GetTransformationFormula")]
        public async Task<IActionResult> GetTransformationFormula()
        {

            var planning = await _unitOfWork.Preparation.GetAllTransformationFormulaInformation();

            return Ok(planning);

        }


        [HttpGet]
        [Route("GetAllStocks")]
        public async Task<IActionResult> GetAllStocks()
        {

            var planning = await _unitOfWork.Preparation.GetAllAvailableStocks();

            return Ok(planning);

        }

        [HttpGet]
        [Route("GetRemaining")]
        public async Task<IActionResult> GetRemainingSample([FromQuery] string itemcode)
        {

            var planning = await _unitOfWork.Preparation.GetAllRemainingStocksPerReceivingId(itemcode);

            return Ok(planning);

        }

    }
}
