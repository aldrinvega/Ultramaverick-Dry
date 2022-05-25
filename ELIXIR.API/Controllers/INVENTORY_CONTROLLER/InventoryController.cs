using ELIXIR.DATA.CORE.ICONFIGURATION;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.INVENTORY_CONTROLLER
{
   
    public class InventoryController : BaseApiController
    {
        private IUnitOfWork _unitOfWork;

        public InventoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

      
        [HttpGet]
        [Route("RawmaterialInventory")]
        public async Task<IActionResult> GetAllAvailableRawmaterial()
        {
            var rawmaterial = await _unitOfWork.Inventory.GetAllAvailbleInRawmaterialInventory();

            return Ok(rawmaterial);
        }


    }
}
